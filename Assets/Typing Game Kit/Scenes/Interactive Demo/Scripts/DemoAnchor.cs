using UnityEngine;

namespace TypingGameKit.Demo
{
    public class DemoAnchor : MonoBehaviour
    {
        [SerializeField] private Color[] _colors = null;

        private static int _spawnCount = 0;

        private void Start()
        {
            var renderer = GetComponent<MeshRenderer>();
            renderer.material.color = _colors[_spawnCount % _colors.Length];
            _spawnCount += 1;
        }
    }
}