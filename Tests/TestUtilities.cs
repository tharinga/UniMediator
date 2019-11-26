using NUnit.Framework;
using UniMediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TestUtilities
    {
        static Scene _scene;

        public static void SetUp()
        {
            if (_scene == default(Scene))
            {
                _scene = SceneManager.CreateScene("TestScene");
                SceneManager.SetActiveScene(_scene);
            }
        }

        [TearDown]
        public static void TearDown()
        {
            foreach (var gameObject in _scene.GetRootGameObjects())
            {
                Object.DestroyImmediate(gameObject);
            }
            Object.DestroyImmediate(Object.FindObjectOfType<MediatorImpl>()?.gameObject);
        }
    }
}