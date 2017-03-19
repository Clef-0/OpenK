using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject
{
    public partial class KProject : Game
    {
        /// <summary>
        /// Learned how to do this using RB Whitaker's "Picking" tutorial
        /// </summary>
        float? RayDistance(BoundingSphere sphere, Vector2 mouseLocation, Matrix view, Matrix projection, Viewport viewport)
        {
            Vector3 point1 = viewport.Unproject(new Vector3(mouseLocation.X, mouseLocation.Y, 0.0f), projection, view, Matrix.Identity);
            Vector3 point2 = viewport.Unproject(new Vector3(mouseLocation.X, mouseLocation.Y, 1.0f), projection, view, Matrix.Identity);
            Vector3 rayVector = point2 - point1;
            rayVector.Normalize();
            return new Ray(point1, rayVector).Intersects(sphere);
        }

        /// <summary>
        /// Calls RayDistance for every mesh in the model passed to determine if the mouse cursor is over it.
        /// </summary>
        bool CheckRay(Vector2 mouseLocation, Model model, Matrix world, Matrix view, Matrix projection, Viewport viewport)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                if (RayDistance(mesh.BoundingSphere.Transform(world), mouseLocation, view, projection, viewport) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
