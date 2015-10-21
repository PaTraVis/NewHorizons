using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BEPUphysics.Entities;

namespace BEPUMono
{
    public class EntityModel : DrawableGameComponent, IDrawableComponent
    {
        public Player Player { get; set; }

        /// <summary>
        /// Entity that this model follows.
        /// </summary>
        Entity entity;
        Model model;
        /// <summary>
        /// Base transformation to apply to the model.
        /// </summary>
        public Matrix Transform;
        Matrix[] boneTransforms;
	    private Texture2D texture;


        /// <summary>
        /// Creates a new EntityModel.
        /// </summary>
        /// <param name="entity">Entity to attach the graphical representation to.</param>
        /// <param name="model">Graphical representation to use for the entity.</param>
        /// <param name="transform">Base transformation to apply to the model before moving to the entity.</param>
        /// <param name="game">Game to which this component will belong.</param>
        public EntityModel(Entity entity, Model model, Matrix transform, Game game, Player player)
            : base(game)
        {
            this.entity = entity;
            this.model = model;
            Transform = transform;
	        Player = player;

	        //Collect any bone transformations in the model itself.
            //The default cube model doesn't have any, but this allows the EntityModel to work with more complicated shapes.
            boneTransforms = new Matrix[model.Bones.Count];
            foreach (var effect in model.Meshes.SelectMany(mesh => mesh.Effects).Cast<BasicEffect>())
            {
                effect.EnableDefaultLighting();
            }
        }

	    public override void Draw(GameTime gameTime)
        {
            //Notice that the entity's worldTransform property is being accessed here.
            //This property is returns a rigid transformation representing the orientation
            //and translation of the entity combined.
            //There are a variety of properties available in the entity, try looking around
            //in the list to familiarize yourself with it.
            var worldMatrix = Transform * MathConverter.Convert(entity.WorldTransform);


            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects.Cast<BasicEffect>())
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = MathConverter.Convert(Player.Camera.ViewMatrix);
                    effect.Projection = MathConverter.Convert(Player.Camera.ProjectionMatrix);
                    effect.LightingEnabled = true; // turn on the lighting subsystem.

                    //effect.Alpha = 1.0f;
                    //effect.DiffuseColor = Color.White.ToVector3();
                   // effect.SpecularColor = Color.White.ToVector3();
                    //effect.SpecularPower = 2.0f;

                    if (texture != null)
                    {
                        effect.Texture = texture;
                        effect.TextureEnabled = true;
                    }
                    else
                    {

                        effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();// a red light
                        effect.DirectionalLight0.Direction = new Vector3(1, 1, 0);  // coming along the x-axis
                        effect.DirectionalLight0.SpecularColor = Color.Yellow.ToVector3(); // with green highlights
                        effect.DirectionalLight0.Enabled = true;
                        
                        effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                        effect.EmissiveColor = new Vector3(1, 0, 0);
                    }
                }
                mesh.Draw();
            }
	        base.Draw(gameTime);
        }
    }
}
