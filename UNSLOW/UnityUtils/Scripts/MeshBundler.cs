using UnityEngine;

namespace UNSLOW.UnityUtils
{
    public class MeshBundler : MonoBehaviour
    {
        [SerializeField] private GameObject fbx;

        public Mesh[] meshes;

        public int Length => meshes.Length;

        public Mesh GetMesh( int index )
        {
            return meshes[index];
        }

        private void OnValidate()
        {
            meshes = fbx.transform.GetComponents<Mesh>();
            Debug.Log( "Mesh:" + meshes.Length );
        }
    }
}
