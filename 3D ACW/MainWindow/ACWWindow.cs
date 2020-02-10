using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using ThreeD_ACW.GameModels;
using ThreeD_ACW.Utility;
using ThreeD_ACW.GameTextures;
using ThreeD_ACW.GameCameras;
using ThreeD_ACW.GameAnimation;
using ThreeD_ACW.GameLighting;
using ThreeD_ACW.GameMaterials;
using ThreeD_ACW.GameFramebuffer;

namespace ThreeD_ACW
{
    public class ACWWindow : GameWindow
    {
        public ACWWindow() : base(
            800, //Width
            800, //Height
            GraphicsMode.Default,
            "3D ACW",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            3, // major
            3, // minor
            GraphicsContextFlags.ForwardCompatible
            )
        {
            
        }

        private int mCurrentCamera = 0;
        private List<Model> mModels = new List<Model>();
        private Dictionary<string, Texture> mTextures = new Dictionary<string, Texture>();
        private Dictionary<string, Material> mMaterials = new Dictionary<string, Material>();
        private Dictionary<string, ModelUtility> mModelUtilities = new Dictionary<string, ModelUtility>();
        private Dictionary<string, ShaderUtility> mShaders = new Dictionary<string, ShaderUtility>();
        private List<Camera> mCameras = new List<Camera>();
        private List<Lighting> mLighting = new List<Lighting>();
        private bool mAnimation = true;
        private Framebuffer mSimpleFrameBuffer;
        private ShaderUtility mSimpleFrameBufferShader;
        private int mEffect = 0;
        private DateTime mCurrent = DateTime.Now;
        private int mShowTexture = 0;

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.White);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            LoadFrameBuffer();
            LoadShaders();
            Console.WriteLine("Shaders Loaded: " + DateTime.Now.ToString());

            LoadTextures();
            Console.WriteLine("Textures Loaded: " + DateTime.Now.ToString());

            LoadMaterials();
            Console.WriteLine("Materials Loaded: " + DateTime.Now.ToString());

            LoadModelUtilities();
            Console.WriteLine("Model Loaded: " + DateTime.Now.ToString());

            LoadModels();
            LoadLights();
            LoadCameras();

            base.OnLoad(e);
        }

        /// <summary>
        /// Loads in the cameras
        /// </summary>
        private void LoadCameras()
        {
            MovingCamera camera1 = new MovingCamera(
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 15),
                new Vector3(0, 1, 0));

            StaticCamera camera2 = new StaticCamera(
                new Vector3(5, 5, 5),
                new Vector3(-5, -5, 15),
                new Vector3(-5, 15, 15));

            StaticCamera camera3 = new StaticCamera(
                new Vector3(-5, 5, 5),
                new Vector3(5, -5, 15),
                new Vector3(5, 15, 15));

            mCameras.Add(camera1);
            mCameras.Add(camera2);
            mCameras.Add(camera3);

            if (mCameras.Count == 0)
            {
                throw new Exception("At least one camera is required");
            }

            ShaderCalls("Lights");
            ShaderCalls("Camera");
        }

        /// <summary>
        /// Loads in the lights
        /// </summary>
        private void LoadLights()
        {
            mLighting.Add(new PointLighting(
                new Vector3(0, 2.5f, -10),
                new Vector3(1, 1, 1), //1, 1,1 
                0.03f));
            mLighting.Add(new SpotLighting(
                new Vector3(-4f, 2.5f, -14f),
                new Vector3(1, 0, 0), //1, 0, 0
                new Vector3(0, 1, 0),
                30,
                0.05f));
            mLighting.Add(new SpotLighting(
                new Vector3(4f, 2.5f, -14f),
                new Vector3(0, 1, 0), // 0,1,0
                new Vector3(0, 1, 0),
                30,
                0.05f));
            mLighting.Add(new SpotLighting(
                new Vector3(0, 4, -10f),
                new Vector3(0, 0, 1), //0,0,1
                new Vector3(0, 1, 0),
                30,
                0.005f));
            mLighting.Add(new DirectionalLighting(
                new Vector3(0, 0, 1),
                new Vector3(0.1f, 0.1f, 0)
                ));
        }

        /// <summary>
        /// Loads in the models with textures, transformation, animations and materials
        /// </summary>
        private void LoadModels()
        {
            Model left = new Model(mModelUtilities["Wall"], mShaders["Textured"].ShaderProgramID);
            left.ApplyTransformation(Matrix4.CreateRotationY((float)(Math.PI / 2)));
            left.ApplyTransformation(Matrix4.CreateTranslation(-5, 0, -10));
            left.ApplyTexture(mTextures["Wall"]);
            left.ApplyMaterial(mMaterials["Walls"]);
            mModels.Add(left);

            Model back = new Model(mModelUtilities["Wall"], mShaders["Material"].ShaderProgramID);
            back.ApplyTransformation(Matrix4.CreateTranslation(0, 0, -15f));
            back.ApplyMaterial(mMaterials["Back"]);
            mModels.Add(back);

            Model front = new Model(mModelUtilities["Wall"], mShaders["Textured"].ShaderProgramID);
            front.ApplyTransformation(Matrix4.CreateRotationY((float)Math.PI));
            front.ApplyTransformation(Matrix4.CreateTranslation(0, 0, -5f));
            front.ApplyTexture(mTextures["Wall"]);
            front.ApplyMaterial(mMaterials["Walls"]);
            mModels.Add(front);

            Model right = new Model(mModelUtilities["Wall"], mShaders["Textured"].ShaderProgramID);
            right.ApplyTransformation(Matrix4.CreateRotationY(-(float)(Math.PI / 2)));
            right.ApplyTransformation(Matrix4.CreateTranslation(5, 0, -10));
            right.ApplyTexture(mTextures["Wall"]);
            right.ApplyMaterial(mMaterials["Walls"]);
            mModels.Add(right);

            Model top = new Model(mModelUtilities["Wall"], mShaders["Textured"].ShaderProgramID);
            top.ApplyTransformation(Matrix4.CreateRotationX((float)(Math.PI / 2)));
            top.ApplyTransformation(Matrix4.CreateTranslation(0, 5, -10));
            top.ApplyTexture(mTextures["Ceiling"]);
            top.ApplyMaterial(mMaterials["Walls"]);
            mModels.Add(top);

            Model bottom = new Model(mModelUtilities["Wall"], mShaders["Textured"].ShaderProgramID);
            bottom.ApplyTransformation(Matrix4.CreateRotationX(-(float)(Math.PI / 2)));
            bottom.ApplyTransformation(Matrix4.CreateTranslation(0, -5, -10));
            bottom.ApplyTexture(mTextures["Carpet"]);
            bottom.ApplyMaterial(mMaterials["Walls"]);
            mModels.Add(bottom);

            Model cubeLeft = new Model(mModelUtilities["Box"], mShaders["Material"].ShaderProgramID);
            cubeLeft.ApplyTransformation(Matrix4.CreateTranslation(-4, -4, -14));
            cubeLeft.ApplyMaterial(mMaterials["LeftBox"]);
            mModels.Add(cubeLeft);

            Model plane = new Model(mModelUtilities["F16"], mShaders["Textured"].ShaderProgramID);
            plane.ApplyTransformation(Matrix4.CreateRotationY(-(float)(Math.PI / 2)));
            plane.ApplyTransformation(Matrix4.CreateTranslation(0, -2.5f, -10));
            plane.ApplyTexture(mTextures["F16"]);
            plane.ApplyAnimation(new PlaneAnimation());
            plane.ApplyMaterial(mMaterials["Metal"]);
            mModels.Add(plane);

            Model teapotLeft = new Model(mModelUtilities["Teapot"], mShaders["Textured"].ShaderProgramID);
            teapotLeft.ApplyTransformation(Matrix4.CreateScale(0.5f));
            teapotLeft.ApplyTransformation(Matrix4.CreateTranslation(0, 1, 0));
            teapotLeft.ApplyTexture(mTextures["Teapot"]);
            teapotLeft.ApplyAnimation(new TeapotLeftAnimation());
            teapotLeft.ApplyMaterial(mMaterials["Ceramic"]);
            cubeLeft.AddModel(teapotLeft);

            Model cubeRight = new Model(mModelUtilities["Box"], mShaders["Textured"].ShaderProgramID);
            cubeRight.ApplyTransformation(Matrix4.CreateTranslation(4, -4, -14));
            cubeRight.ApplyTexture(mTextures["Box"]);
            cubeRight.ApplyMaterial(mMaterials["Wood"]);
            mModels.Add(cubeRight);

            Model teapotRight = new Model(mModelUtilities["Teapot"], mShaders["Material"].ShaderProgramID);
            teapotRight.ApplyTransformation(Matrix4.CreateScale(0.5f));
            teapotRight.ApplyTransformation(Matrix4.CreateTranslation(0, 1, 0));
            teapotRight.ApplyAnimation(new TeapotRightAnimation());
            teapotRight.ApplyMaterial(mMaterials["Ceramic"]);
            cubeRight.AddModel(teapotRight);

            Model sun = new Model(mModelUtilities["Sphere"], mShaders["Sun"].ShaderProgramID);
            sun.ApplyTransformation(Matrix4.CreateScale(0.5f));
            sun.ApplyTransformation(Matrix4.CreateTranslation(0, 2.5f, -10));
            sun.ApplyAnimation(new PlanetAnimation());
            sun.ApplyMaterial(mMaterials["Sun"]);
            sun.ApplyTexture(mTextures["Sun"]);
            mModels.Add(sun);

            Model earth = new Model(mModelUtilities["Sphere"], mShaders["Textured"].ShaderProgramID);
            earth.ApplyTransformation(Matrix4.CreateScale(0.5f));
            earth.ApplyTransformation(Matrix4.CreateTranslation(-5, 0, 0));
            earth.ApplyTexture(mTextures["Earth"]);
            earth.ApplyAnimation(new PlanetAnimation());
            earth.ApplyMaterial(mMaterials["Planet"]);
            sun.AddModel(earth);

            Model moon = new Model(mModelUtilities["Sphere"], mShaders["Textured"].ShaderProgramID);
            moon.ApplyTransformation(Matrix4.CreateScale(0.5f));
            moon.ApplyTransformation(Matrix4.CreateTranslation(-5, 0, 0));
            moon.ApplyTexture(mTextures["Moon"]);
            moon.ApplyMaterial(mMaterials["Planet"]);
            earth.AddModel(moon);

            Model dragon = new Model(mModelUtilities["Dragonhead"], mShaders["Textured"].ShaderProgramID);
            dragon.ApplyTransformation(Matrix4.CreateScale(0.001f));
            dragon.ApplyTransformation(Matrix4.CreateTranslation(0, 0, -14.25f));
            dragon.ApplyMaterial(mMaterials["Ceramic"]);
            dragon.ApplyTexture(mTextures["Dragon"]);
            mModels.Add(dragon);
        }

        /// <summary>
        /// Loads in the model data
        /// </summary>
        private void LoadModelUtilities()
        {
            mModelUtilities.Add("Wall", ModelUtility.LoadModel(@"Utility/Models/Primitive/side.sjg"));
            mModelUtilities.Add("Box", ModelUtility.LoadModel(@"Utility/Models/Primitive/cube.sjg"));
            mModelUtilities.Add("F16", ModelUtility.LoadModel(@"Utility/Models/Complex/f16.obj"));
            mModelUtilities.Add("Teapot", ModelUtility.LoadModel(@"Utility/Models/Complex/Teapot.obj"));
            mModelUtilities.Add("Sphere", ModelUtility.LoadModel(@"Utility/Models/Primitive/Sphere.obj"));
            mModelUtilities.Add("Dragonhead", ModelUtility.LoadModel(@"Utility/Models/Complex/Dragonhead.obj"));
        }

        /// <summary>
        /// Loads in the mateials data
        /// </summary>
        private void LoadMaterials()
        {
            mMaterials.Add("Walls", new Material(
                new Vector4(0.1f, 0.1f, 0.1f, 1),
                new Vector4(0.9f, 0.9f, 0.9f, 1),
                new Vector4(0.0f, 0.0f, 0.0f, 1),
                0.01f));
            mMaterials.Add("Ceramic", new Material(
                new Vector4(0.1f, 0.1f, 0.1f, 1),
                new Vector4(1f, 1f, 1f, 1),
                new Vector4(1, 1, 1, 1),
                256));
            mMaterials.Add("LeftBox", new Material(
                new Vector4(0.329412f, 0.223529f, 0.027451f, 1),
                new Vector4(0.780392f, 0.568627f, 0.113725f, 1),
                new Vector4(0.992157f, 0.941176f, 0.807843f, 1),
                27.8974f));
            mMaterials.Add("Wood", new Material(
                new Vector4(0.1f, 0.1f, 0.1f, 1),
                new Vector4(0.9f, 0.9f, 0.9f, 1),
                new Vector4(0.1f, 0.1f, 0.1f, 1),
                1));
            mMaterials.Add("Back", new Material(
                new Vector4(0.02f, 0.02f, 0.02f, 1),
                new Vector4(0.01f, 0.01f, 0.01f, 1),
                new Vector4(0.4f, 0.4f, 0.4f, 1),
                10));
            mMaterials.Add("Metal", new Material(
                new Vector4(0.19225f, 0.19225f, 0.19225f, 1),
                new Vector4(0.50754f, 0.50754f, 0.50754f, 1),
                new Vector4(0.508273f, 0.508273f, 0.508273f, 1),
                50));
            mMaterials.Add("Sun", new Material(
                new Vector4(0.1f, 0.1f, 0.1f, 1),
                new Vector4(0.9f, 0.9f, 0.9f, 1),
                new Vector4(0.0f, 0.0f, 0.0f, 1),
                0.01f));
            mMaterials.Add("Planet", new Material(
                new Vector4(0.25f, 0.25f, 0.25f, 1),
                new Vector4(1f, 1f, 1f, 1),
                new Vector4(1, 1, 1, 1),
                100));
        }

        /// <summary>
        /// Loads in the frambufffer and post processing shader
        /// </summary>
        private void LoadFrameBuffer()
        {
            mSimpleFrameBufferShader = new ShaderUtility(@"Utility/Shaders/SecondPass/vPost.vert", @"Utility/Shaders/SecondPass/fPost.frag");
            mSimpleFrameBuffer = new Framebuffer(mSimpleFrameBufferShader);
        }

        /// <summary>
        /// Loads in the shaders
        /// </summary>
        private void LoadShaders()
        {
            mShaders.Add("Textured", new ShaderUtility(@"Utility/Shaders/FirstPass/vTexture.vert", @"Utility/Shaders/FirstPass/fTexture.frag"));
            mShaders.Add("Material", new ShaderUtility(@"Utility/Shaders/FirstPass/vMaterial.vert", @"Utility/Shaders/FirstPass/fMaterial.frag"));
            mShaders.Add("Sun", new ShaderUtility(@"Utility/Shaders/FirstPass/vSun.vert", @"Utility/Shaders/FirstPass/fSun.frag"));
        }

        /// <summary>
        /// Loads in the textures
        /// </summary>
        private void LoadTextures()
        {
            mTextures.Add("Wall", new Texture(@"Utility/Textures/Wall_Texture.jpg", @"Utility/Textures/Wall_Normal.jpg"));
            mTextures.Add("Carpet", new Texture(@"Utility/Textures/Carpet_Texture.jpg", @"Utility/Textures/Carpet_Normal.jpg"));
            mTextures.Add("Ceiling", new Texture(@"Utility/Textures/Ceiling_Texture.jpg", @"Utility/Textures/Ceiling_Normal.jpg"));
            mTextures.Add("F16", new Texture(@"Utility/Textures/F16_Texture.jpg", @"Utility/Textures/F16_Normal.jpg"));
            mTextures.Add("Box", new Texture(@"Utility/Textures/BoxSide_Texture.jpg", @"Utility/Textures/BoxSide_Normal.jpg"));
            mTextures.Add("Teapot", new Texture(@"Utility/Textures/Teapot_Texture.jpg", @"Utility/Textures/Teapot_Normal.jpg"));
            mTextures.Add("Earth", new Texture(@"Utility/Textures/Earth_Texture.jpg", @"Utility/Textures/Earth_Normal.jpg"));
            mTextures.Add("Moon", new Texture(@"Utility/Textures/Moon_Texture.jpg", @"Utility/Textures/Moon_Normal.jpg"));
            mTextures.Add("Dragon", new Texture(@"Utility/Textures/Dragon_Texture.jpg", @"Utility/Textures/Dragon_Normal.jpg"));
            mTextures.Add("Sun", new Texture(@"Utility/Textures/Sun_Texture.jpg", ""));
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            ShaderCalls("Resize"); //Calls resize of each of the shaders
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == 'c')
            {
                if(++mCurrentCamera >= mCameras.Count) { //Changes the current camera
                    mCurrentCamera = 0;
                }
            }
            else if(e.KeyChar == 'p') //Pauses animations
            {
                mAnimation = !mAnimation;
            }
            else if(e.KeyChar == 'f') //Changes post-processing effect
            {
                if(++mEffect > 5)
                {
                    mEffect = 0;
                }
            }
            else if(e.KeyChar == 't') //Shows colour texture or normal map only
            {
                if(++mShowTexture > 1)
                {
                    mShowTexture = 0;
                }
                ShaderCalls("Texture");
            }
            else
            {
                mCameras[mCurrentCamera].MoveCamera(e); //Moves the camera (does nothing if static light)
            }
            ShaderCalls("Camera"); //Updates the camere view matrix and eye position
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            mSimpleFrameBuffer.FirstPassStart(); //Allows for rending to texture
            GL.Viewport(0, 0, ClientRectangle.Width, ClientRectangle.Height);

            for (int i = 0; i < mModels.Count; i++)
            {
                mModels[i].Draw(Matrix4.Identity);
            }

            mSimpleFrameBuffer.SecondPass(mSimpleFrameBufferShader, ClientRectangle, mEffect); //Renders the created texture onto the screen

            GL.BindVertexArray(0);

            this.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if(mAnimation) //Calls animation on all models (not paused)
            {
                for (int i = 0; i < mModels.Count; i++)
                {
                    mModels[i].Animate();
                }
            }
            base.OnUpdateFrame(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //Deletes created data from the GPU
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            foreach (KeyValuePair<string, Texture> texture in mTextures)
            {
                texture.Value.Delete();
            }

            for (int i = 0; i < mModels.Count; i++)
            {
                mModels[i].Delete();
            }

            ShaderCalls("Delete");

            mSimpleFrameBuffer.Delete();
            base.OnUnload(e);
        }

        private void ShaderCalls(string pCall)
        {
            foreach(KeyValuePair<string, ShaderUtility> shader in mShaders) //For each shader
            {
                GL.UseProgram(shader.Value.ShaderProgramID);

                switch(pCall)
                {
                    case "Delete":
                        shader.Value.Delete(); //Delete shader from GPU
                        break;
                    case "Camera":
                        mCameras[mCurrentCamera].UseCamera(shader.Value); //Update view and eye position
                        break;
                    case "Lights":
                        for (int i = 0; i < mLighting.Count; i++) //Place lights into the correct values
                        {
                            mLighting[i].UseLight(i, shader.Value);
                        }
                        break;
                    case "Texture":
                        int uShowTextureLocation = GL.GetUniformLocation(shader.Value.ShaderProgramID, "uShowTexture"); //Show texture with colour or normal map only
                        GL.Uniform1(uShowTextureLocation, mShowTexture);
                        break;
                    case "Resize": //Updates the pprojection matrix when the client resizes
                        if (shader.Value != null)
                        {
                            int uProjectionLocation = GL.GetUniformLocation(shader.Value.ShaderProgramID, "uProjection");
                            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 20);
                            GL.UniformMatrix4(uProjectionLocation, true, ref projection);
                        }
                        break;
                }
            }
        }
    }
}
