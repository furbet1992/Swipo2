using UnityEngine;

namespace TypingGameKit.BasicSetup
{
    /// <summary>
    /// This is a simple script to illustrate how sequences can be attached to scene objects with the Dynamic Overlay system.
    /// </summary>
    public class SequenceOverlayExample : MonoBehaviour
    {
        // The radius around the scene's origin in which to spawn the objects
        [SerializeField] private float _spawnRadius = 5;

        // The number of objects with sequences to spawn.
        [SerializeField] private float _objectCount = 10;

        // A reference to a spawner that can spawn sequences
        [SerializeField] private SequenceOverlay _sequenceOverlay = null;

        // A reference to a text collection that is used to construct required input.
        [SerializeField] private TextCollection _texts = null;

        private void Start()
        {
            // Spawn the initial sequences!
            for (int i = 0; i < _objectCount; i++)
            {
                CreateObjectWithSequence();
            }

            // If any sequence is completed create a new one.
            InputSequenceManager.OnSequenceCompleted += delegate { CreateObjectWithSequence(); };
        }

        // Creates
        private void CreateObjectWithSequence()
        {
            // Creates the a object somewhere in the scene
            GameObject sceneObject = CrateObjectInScene();

            // Add sequence to object
            AddSequenceToObject(sceneObject);
        }

        private void AddSequenceToObject(GameObject sceneObject)
        {
            // Generate a text sequence with a unique among the other sequences active sequences
            string text = _texts.FindUniquelyTargetableText();

            // Crate a sequence with the given text and object to follow in the scene.
            InputSequence sequence = _sequenceOverlay.CreateSequence(text, sceneObject.transform);

            // Let's make it so that the object is destroyed when the sequence is completed.
            sequence.OnCompleted += delegate { Destroy(sceneObject); };
        }

        private GameObject CrateObjectInScene()
        {
            GameObject obj = new GameObject("Target Object");
            obj.transform.SetParent(transform);
            obj.transform.position = Random.insideUnitSphere * _spawnRadius;
            return obj;
        }
    }
}