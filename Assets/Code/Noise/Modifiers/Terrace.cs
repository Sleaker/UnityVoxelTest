using System;
using Voxel.Noise.Util;

namespace Voxel.Noise.Modifiers
{
    public class Terrace : NoiseModule
    {
	    /// Determines if the terrace-forming curve between all control points
	    /// is inverted.
	    public bool InvertTerraces
        {
            get;
            set;
        }


        public NoiseModule SourceModule
        {
            get;
            set;
        }

	    /// Array that stores the control points.
	    double[] ControlPoints = new double[0];

	    public Terrace() {
		   
	    }
        	  
	    public int GetControlPointCount() 
        {
		    return ControlPoints.Length;
	    }

	    public double[] GetControlPoints() 
        {
		    return ControlPoints;
	    }

	    public void AddControlPoint(double value) 
        {
		    int insertionPos = FindInsertionPos(value);
		    InsertAtPos(insertionPos, value);
	    }

	    public void ClearAllControlPoints() 
        {
		    ControlPoints = new double[0];
	    }

	    public void MakeControlPoints(int controlPointCount) 
        {
		    if (controlPointCount < 2) 
            {
			    throw new ArgumentException("Must have more than 2 control points");
		    }

		    ClearAllControlPoints();

		    double terraceStep = 2.0 / (controlPointCount - 1.0);
		    double curValue = -1.0;
		    for (int i = 0; i < controlPointCount; i++) 
            {
			    AddControlPoint(curValue);
			    curValue += terraceStep;
		    }

	    }

	    protected int FindInsertionPos(double value) 
        {
		    int insertionPos;
		    for (insertionPos = 0; insertionPos < ControlPoints.Length; insertionPos++)
		    {
		        if (value < ControlPoints[insertionPos]) 
                {
				    // We found the array index in which to insert the new control point.
				    // Exit now.
				    break;
			    }
		        if (value == ControlPoints[insertionPos])
		        {
		            // Each control point is required to contain a unique value, so throw
		            // an exception.
		            throw new ArgumentException("Value must be unique");
		        }
		    }
	        return insertionPos;

	    }

	    protected void InsertAtPos(int insertionPos, double value) 
        {
		    // Make room for the new control point at the specified position within
		    // the control point array.  The position is determined by the value of
		    // the control point; the control points must be sorted by value within
		    // that array.
		    double[] newControlPoints = new double[ControlPoints.Length + 1];
            for (int i = 0; i < ControlPoints.Length; i++) 
            {
			    if (i < insertionPos) 
                {
				    newControlPoints[i] = ControlPoints[i];
			    } 
                else 
                {
				    newControlPoints[i + 1] = ControlPoints[i];
			    }
		    }

		    ControlPoints = newControlPoints;
		 
		    // Now that we've made room for the new control point within the array,
		    // add the new control point.
		    ControlPoints[insertionPos] = value;

	    }
	    
	    public override double GetValue(double x, double y, double z) 
        {
            if (SourceModule == null)
                throw new InvalidOperationException("Source Module cannot be null");

		    // Get the output value from the source module.
		    double sourceModuleValue = SourceModule.GetValue(x, y, z);

		    // Find the first element in the control point array that has a value
		    // larger than the output value from the source module.
		    int indexPos;
            for (indexPos = 0; indexPos < ControlPoints.Length; indexPos++)
            {
			    if (sourceModuleValue < ControlPoints[indexPos]) {
				    break;
			    }
		    }

		    // Find the two nearest control points so that we can map their values
		    // onto a quadratic curve.
            int index0 = NoiseMath.ClampValue(indexPos - 1, 0, ControlPoints.Length - 1);
            int index1 = NoiseMath.ClampValue(indexPos, 0, ControlPoints.Length - 1);

		    // If some control points are missing (which occurs if the output value from
		    // the source module is greater than the largest value or less than the
		    // smallest value of the control point array), get the value of the nearest
		    // control point and exit now.
		    if (index0 == index1) {
			    return ControlPoints[index1];
		    }

		    // Compute the alpha value used for linear interpolation.
		    double value0 = ControlPoints[index0];
		    double value1 = ControlPoints[index1];
		    double alpha = (sourceModuleValue - value0) / (value1 - value0);
		    if (InvertTerraces) {
			    alpha = 1.0 - alpha;
			    double temp = value0;
			    value0 = value1;
			    value1 = temp;
		    }

		    // Squaring the alpha produces the terrace effect.
		    alpha *= alpha;

		    // Now perform the linear interpolation given the alpha value.
            return NoiseMath.LinearInterpolate(value0, value1, alpha);
	    }
    }
}
