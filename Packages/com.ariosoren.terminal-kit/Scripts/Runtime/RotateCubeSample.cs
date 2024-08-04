using UnityEngine;

namespace ArioSoren.TerminalKit
{
    public class RotateCubeSample : MonoBehaviour
    {
        private bool _stop;
        void Update()
        {
            if (!_stop)
                transform.Rotate(new Vector3(1, 1, 1));
        }


        [TerminalCommand("stop-cube", "stops the cube from rotating")]
        public void StopCube()
        {
            _stop = true;
        }


        [TerminalCommand("rotate-cube", "rotates the cube")]
        public void RotateTheCube()
        {
            _stop = false;
        }

        [TerminalCommand("move-cube", "move-cube(x,y,z) Moves the cube")]
        public void Move(float x, float y, float z)
        {
            transform.position = new Vector3(x, y, z);
        }
        [TerminalCommand("color-cube", "color-cube(x,y,z) Moves the cube")]
        public void Color(float r, float g, float b)
        {
            var cubeRenderer = GetComponent<Renderer>();
            Color cubeColor = new Color(r, g, b);
            cubeRenderer.material.SetColor("_Color",cubeColor);
        }
    
        [TerminalCommand("changeColor", "changeColor")]
        public void ChangeColor()
        {
            var cubeRenderer = GetComponent<Renderer>();
            Color cubeColor = new Color(Random.Range(0,1), Random.Range(0,1), Random.Range(0,1));
            cubeRenderer.material.SetColor("_Color",cubeColor);
        }
    }
}
