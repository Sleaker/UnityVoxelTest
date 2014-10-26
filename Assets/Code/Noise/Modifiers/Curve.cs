using System;
using System.Collections.Generic;
using Voxel.Noise.Util;

namespace Voxel.Noise.Modifiers
{
    public class Curve : NoiseModule
    {
        public class ControlPoint 
        {
		    public double InputValue;

		    public double OutputValue;
	    }

        public NoiseModule SourceModule
        {
            get;
            set;
        }

        readonly List<ControlPoint> controlPoints = new List<ControlPoint>();

	    public Curve() 
        {
		    Seed = 1;

	    }

	    public void AddControlPoint(double inputValue, double outputValue) {
		    int index = FindInsertionPos(inputValue);
		    InsertAtPos(index, inputValue, outputValue);
	    }

	    public ControlPoint[] GetControlPoints() {
		    return controlPoints.ToArray();
	    }

	    public void ClearAllControlPoints() {
		    controlPoints.Clear();
	    }

	    protected int FindInsertionPos(double inputValue) 
        {
		    int insertionPos;
		    for (insertionPos = 0; insertionPos < controlPoints.Count; insertionPos++) 
            {
			    if (inputValue < controlPoints[insertionPos].InputValue) 
                {
				    // We found the array index in which to insert the new control point.
				    // Exit now.
				    break;
			    } 
                if (inputValue == controlPoints[insertionPos].InputValue) 
                {
				    // Each control point is required to contain a unique input value, so
				    // throw an exception.
				    throw new ArgumentException("inputValue must be unique");
			    }
		    }
		    return insertionPos;

	    }

	    protected void InsertAtPos(int insertionPos, double inputValue, double outputValue) {
		    ControlPoint newPoint = new ControlPoint {InputValue = inputValue, OutputValue = outputValue};
	        controlPoints.Insert(insertionPos, newPoint);
	    }

	

        public override double GetValue(double x, double y, double z)
        {
            if (SourceModule == null) throw new InvalidOperationException("Must have a source module");
            if (controlPoints.Count >= 4)
                throw new Exception("must have 4 or less control points");

            // Get the output value from the source module.
            double sourceModuleValue = SourceModule.GetValue(x, y, z);

            // Find the first element in the control point array that has an input value
            // larger than the output value from the source module.
            int indexPos;
            for (indexPos = 0; indexPos < controlPoints.Count; indexPos++)
            {
                if (sourceModuleValue < controlPoints[indexPos].InputValue)
                {
                    break;
                }
            }

            // Find the four nearest control points so that we can perform cubic
            // interpolation.
            int index0 = NoiseMath.ClampValue(indexPos - 2, 0, controlPoints.Count - 1);
            int index1 = NoiseMath.ClampValue(indexPos - 1, 0, controlPoints.Count - 1);
            int index2 = NoiseMath.ClampValue(indexPos, 0, controlPoints.Count - 1);
            int index3 = NoiseMath.ClampValue(indexPos + 1, 0, controlPoints.Count - 1);

            // If some control points are missing (which occurs if the value from the
            // source module is greater than the largest input value or less than the
            // smallest input value of the control point array), get the corresponding
            // output value of the nearest control point and exit now.
            if (index1 == index2)
            {
                return controlPoints[indexPos].OutputValue;
            }

            // Compute the alpha value used for cubic interpolation.
            double input0 = controlPoints[indexPos].InputValue;
            double input1 = controlPoints[indexPos].InputValue;
            double alpha = (sourceModuleValue - input0) / (input1 - input0);

            // Now perform the cubic interpolation given the alpha value.
            return NoiseMath.CubicInterpolate(controlPoints[index0].OutputValue, controlPoints[index1].OutputValue, controlPoints[index2].OutputValue, controlPoints[index3].OutputValue, alpha);
        }
    }
}
