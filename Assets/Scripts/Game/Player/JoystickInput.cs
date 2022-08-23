using Game.Player.Interfaces;
using UnityEngine;

namespace Game.Player
{
    public class JoystickInput : IJoystickInput
    {
        public JoystickInput(Joystick joystick)
        {
            _joystick = joystick;
        }
        
        private readonly Joystick _joystick;

        public Vector2 Direction => _joystick.Direction;
    }
}