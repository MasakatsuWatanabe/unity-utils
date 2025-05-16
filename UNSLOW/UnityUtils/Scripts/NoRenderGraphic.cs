using UnityEngine.UI;

namespace UNSLOW.UnityUtils
{
    /// <summary>
    /// 描画しないUIオブジェクト ヒットだけは得られる
    /// </summary>
    public class NoRenderGraphic : Graphic
    {
        public override void SetMaterialDirty()
        {
            // Nothing to do.
        }

        public override void SetVerticesDirty()
        {
            // Nothing to do.
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
