using TMPro;
using UnityEngine;

namespace MyGame.Utilities
{
    public class TextMeshAnim : MonoBehaviour
    {
        private TMP_Text _text;
        private Mesh _textMesh;
        private Vector3[] _textVertices;
        private float _colorHue;
        public float speed = 1f;

        public bool wobble;
        public bool colorLerp;
        void Start()
        {
            _text = GetComponent<TMP_Text>();
        }
        void Update()
        {
            if (wobble)
            {
                _text.ForceMeshUpdate();
                _textMesh = _text.mesh;
                _textVertices = _textMesh.vertices;

                for (var i = 0; i < _text.textInfo.characterInfo.Length; i++)
                {
                    TMP_CharacterInfo charsInfo = _text.textInfo.characterInfo[i];
                    Vector3 offset = Wobble(Time.time + i);
                    var index = charsInfo.vertexIndex;
                    _textVertices[index] += offset;
                    _textVertices[index + 1] += offset;
                    _textVertices[index + 2] += offset;
                    _textVertices[index + 3] += offset;
                }

                _textMesh.vertices = _textVertices;
                _text.canvasRenderer.SetMesh(_textMesh);
            }

            if (colorLerp)
            {
                _text.ForceMeshUpdate();
                var textInfo = _text.textInfo;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible)
                        continue;

                    int vertexIndex = charInfo.vertexIndex;
                    var vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

                    float hue = Mathf.PingPong(Time.time * speed + i * 0.1f, 1);
                    Color color = Color.HSVToRGB(hue, 1, 1);

                    textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 0] = color;
                    textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 1] = color;
                    textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 2] = color;
                    textInfo.meshInfo[charInfo.materialReferenceIndex].colors32[vertexIndex + 3] = color;
                }

                _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }
        }

        Vector2 Wobble(float time)
        {
            return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 1.8f));
        }
    }
}
