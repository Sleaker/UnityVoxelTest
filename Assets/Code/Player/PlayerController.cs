using System;
using System.Linq;
using Voxel.Behavior;
using UnityEngine;
using Voxel.Terrain;
using Voxel.Graphics;

namespace Voxel.Player
{
    class PlayerController : ModBehavior
    {
        private CharacterController Controller;
        private PhysicMaterial BouncyMaterial;
        private Camera MainCamera;

        public World World;
        public Vector3 Gravity = new Vector3(0, -9f, 0);
        public Vector3 Velocity = Vector3.zero;
        public Vector3 MaxVelocity = new Vector3(10, 10, 10);
        public String UIStatus = "playing";
        public int MoveSpeed = 5;
        public int JumpImpulse = 8 * 2;
        public int CurrentMesher = 0;
        public float Dampening = .8f;
        public float TimeInAir = .5f;
        public float JumpAccumilator;
        public bool OnGround;

        readonly IMesher[] Meshers =
        {
            new BlockMesher(),
            new MCMesher(),
        };
        public void Start()
        {
            Controller = gameObject.AddComponent<CharacterController>();
            Controller.radius = .25f;
            Controller.height = 1;
            MainCamera = Camera.main;
            MainCamera.nearClipPlane = .1f;
            gameObject.AddComponent<MouseLook>();
            BouncyMaterial = (PhysicMaterial)Resources.Load("BouncySphereMaterial");
        }

        public void Update()
        {
            Vector3 moveVector = transform.TransformDirection(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveVector.y = 0;
            Velocity += moveVector * MoveSpeed;
            Velocity += Gravity;

            float jumpdamp = 1.0f;

            CollisionFlags f = Controller.Move(Velocity * Time.deltaTime);

            OnGround = (f & CollisionFlags.Below) == CollisionFlags.Below;
            if (OnGround)
            {
                Velocity.y = 0;
            }

            Velocity -= Velocity * Dampening;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                switch (UIStatus.ToLowerInvariant())
                {
                    case "playing":
                        UIStatus = "paused";
                        Screen.lockCursor = false;
                        break;
                    case "paused":
                        UIStatus = "playing";
                        Screen.lockCursor = true;
                        break;
                    case "options":
                        UIStatus = "paused";
                        break;
                }
            }

            //if (!UIStatus.EqualsIgnoreCase("playing")) return;
            //Equipment eq = gameObject.GetComponent<Equipment>();
            //UI ui = gameObject.GetComponent<UI>();

            //// Number 1
            //if (Input.GetKeyDown("1"))
            //{
            //    if (ui != null) ui.ActiveSlots.Add(0);
            //    if (eq != null) PlaceBlock(eq.Hotbar, 0);
            //}

            //if (Input.GetKeyUp("1"))
            //{
            //    if (ui != null) ui.ActiveSlots.Remove(0);
            //}

            //// Number 2
            //if (Input.GetKeyDown("2"))
            //{
            //    if (ui != null) ui.ActiveSlots.Add(1);
            //    if (eq != null) PlaceBlock(eq.Hotbar, 1);
            //}

            //if (Input.GetKeyUp("2"))
            //{
            //    if (ui != null) ui.ActiveSlots.Remove(1);
            //}

            //// Number 3
            //if (Input.GetKeyDown("3"))
            //{
            //    if (ui != null) ui.ActiveSlots.Add(2);
            //    if (eq != null) PlaceBlock(eq.Hotbar, 2);
            //}

            //if (Input.GetKeyUp("3"))
            //{
            //    if (ui != null) ui.ActiveSlots.Remove(2);
            //}

            //// Number 4
            //if (Input.GetKeyDown("4"))
            //{
            //    if (ui != null) ui.ActiveSlots.Add(3);
            //    if (eq != null) PlaceBlock(eq.Hotbar, 3);
            //}

            //if (Input.GetKeyUp("4"))
            //{
            //    if (ui != null) ui.ActiveSlots.Remove(3);
            //}

            //// Number 5
            //if (Input.GetKeyDown("5"))
            //{
            //    if (ui != null) ui.ActiveSlots.Add(4);
            //    if (eq != null) PlaceBlock(eq.Hotbar, 4);
            //}

            //if (Input.GetKeyUp("5"))
            //{
            //    if (ui != null) ui.ActiveSlots.Remove(4);
            //}
            
            // Teleport the player to the middle of the voxel grid
            if (Input.GetKeyDown(KeyCode.E))
            {
                transform.position = new Vector3((16.0f * 16.0f) / 2.0f, 20.0f, (16.0f * 16.0f) / 2.0f);
            }

            // Take a screenshot
            if (Input.GetKeyDown(KeyCode.K))
            {
                Game.TakeScreenshot();
            }

            //// Switch Mesher
            //if (Input.GetKeyDown(KeyCode.O))
            //{
            //    CurrentMesher++;
            //    if (CurrentMesher == Meshers.Length) CurrentMesher = 0;
            //    GameObject g = GameObject.FindWithTag("World");
            //    World w = g.GetComponent<World>();
            //    w.Mesher = Meshers[CurrentMesher];
            //}

            // Shoot a sphere
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.transform.position = gameObject.transform.position + (Vector3.up * .7f) + (gameObject.transform.forward * 1.5f);
                ball.AddComponent<Rigidbody>();
                ball.rigidbody.mass = 1;
                ball.rigidbody.AddForce(transform.forward * 20, ForceMode.Impulse);
                ball.collider.material = BouncyMaterial;
                Destroy(ball, 10);
            }

            // Jump
            if (Input.GetButton("Jump"))
            {
                if (OnGround)
                {
                    JumpAccumilator = 0;
                }
                else
                {
                    Velocity += (-Gravity * (1 - jumpdamp));
                }
                jumpdamp = Mathf.Max(Mathf.Lerp(1f, 0, JumpAccumilator / TimeInAir), 0);
                JumpAccumilator += Time.deltaTime;
                Velocity += (new Vector3(0, JumpImpulse, 0) * jumpdamp);
            }

            // Primary
            if (Input.GetButtonDown("Fire1"))
            {
                //if (ui != null) ui.PrimaryActive = true;
                Ray r = new Ray(transform.position + (Vector3.up * .7f), transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, 5))
                {
                    if (hit.collider.gameObject.GetComponent<Chunk>() != null)
                    {
                        World.ChangeBlock(hit.point + (r.direction * .1f), null);
                    }
                }   
            }

            //if (Input.GetButtonUp("Fire1"))
            //{
            //    if (ui != null) ui.PrimaryActive = false;
            //}

            //// Secondary
            //if (Input.GetButtonDown("Fire2"))
            //{
            //    if (ui != null) ui.SecondaryActive = true;
            //}

            //if (Input.GetButtonUp("Fire2"))
            //{
            //    if (ui != null) ui.SecondaryActive = false;
            //}
        }

        public void OnGUI()
        {
            //Equipment equipment = gameObject.GetComponent<Equipment>();
            //UI ui = gameObject.GetComponent<UI>();

            //switch (UIStatus.ToLowerInvariant())
            //{
            //    case "playing":
            //        ui.DrawHotbar();
            //        ui.DrawRectical();
            //        ui.DrawDebug();
            //        break;
            //    case "paused":
            //        ui.DrawHotbar();
            //        ui.DrawDebug();
            //        ui.DrawPauseMenu();
            //        break;
            //    case "options":
            //        ui.DrawHotbar();
            //        ui.DrawDebug();
            //        ui.DrawOptionsMenu();
            //        break;
            //}
        }

        public void LateUpdate()
        {
            MainCamera.transform.position = transform.position + new Vector3(0, .7f, 0);
            MainCamera.transform.rotation = transform.rotation;
        }

        //private void PlaceBlock(Inventory inventory, int slot)
        //{
        //    Ray r = new Ray(transform.position + (Vector3.up * .7f), transform.forward);
        //    RaycastHit hit;
        //    if (Physics.Raycast(r, out hit, 5))
        //    {
        //        if (hit.collider.gameObject.GetComponent<Chunk>() != null)
        //        {
        //            Item item = inventory.ElementAt(slot);
        //            if (!(item is BlockItem)) return;
        //            item.OnUse(ItemUseMode.Primary, gameObject);
        //            if (((StackableItem)item).StackCount <= 0)
        //            {
        //                inventory[0, slot] = null;
        //            }
        //        }
        //    }
        //}
    }
}
