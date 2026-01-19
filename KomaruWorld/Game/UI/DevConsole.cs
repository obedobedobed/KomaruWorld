using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace KomaruWorld
{
    public class DevConsole
    {
        public bool IsOpen { get; private set; }
        
        private Texture2D _pixel;
        private List<string> _log = new List<string>();
        private string _inputBuffer = "";
        private const int HEIGHT = 240;
        private const int LINE_HEIGHT = 18; 
        private KeyboardState _lastKeyboard;

        public DevConsole(GraphicsDevice graphics, GameWindow window)
        {
            _pixel = new Texture2D(graphics, 1, 1);
            _pixel.SetData(new[] { Color.Black * 0.8f });

            window.TextInput += OnTextInput;

            Log("Console Ready.");
            Log("Commands: host [port], join [ip]");
        }

        private void OnTextInput(object sender, TextInputEventArgs e)
        {
            if (!IsOpen) return;
            if (e.Character == '`' || e.Character == '~') return;

            if (e.Key == Keys.Back)
            {
                if (_inputBuffer.Length > 0)
                    _inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1);
                return;
            }

            if (e.Key == Keys.Enter)
            {
                ExecuteCommand(_inputBuffer);
                _inputBuffer = "";
                return;
            }

            // Only allow standard characters
            if (char.IsLetterOrDigit(e.Character) || char.IsPunctuation(e.Character) || char.IsSymbol(e.Character) || e.Character == ' ')
            {
                _inputBuffer += e.Character;
            }
        }

        public void Update()
        {
            var kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.OemTilde) && !_lastKeyboard.IsKeyDown(Keys.OemTilde))
            {
                IsOpen = !IsOpen;
            }
            _lastKeyboard = kstate;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsOpen) return;

            // Draw Background
            spriteBatch.Draw(_pixel, new Rectangle(0, 0, GameParameters.VIRTUAL_WIDTH, HEIGHT), Color.White);

            // Draw Log
            int y = 10;
            int startLine = Math.Max(0, _log.Count - 15);
            for (int i = startLine; i < _log.Count; i++)
            {
                // Use Text.Draw, not DrawString
                Text.Draw(_log[i], new Vector2(10, y), Color.White, spriteBatch, TextDrawingMode.Right);
                y += LINE_HEIGHT;
            }

            // Draw Prompt (Using ':' instead of '>')
            string prompt = ": " + _inputBuffer + "_";
            Text.Draw(prompt, new Vector2(10, HEIGHT - 20), Color.Yellow, spriteBatch, TextDrawingMode.Right);
        }

        public void Log(string message)
        {
            _log.Add(message);
            if (_log.Count > 50) _log.RemoveAt(0);
        }

        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return;
            Log(": " + command); // Echo using ':'

            string[] parts = command.Split(' ');
            string cmd = parts[0].ToLower();

            try 
            {
                if (cmd == "host")
                {
                    int port = 9050;
                    if (parts.Length > 1) int.TryParse(parts[1], out port);
                    Game1.Instance.NetworkManager.StartHost(port);
                    Log("Host started.");
                }
                else if (cmd == "join")
                {
                    string ip = "localhost";
                    int port = 9050;
                    if (parts.Length > 1) ip = parts[1].Trim();
                    if (parts.Length > 2) int.TryParse(parts[2], out port);
                    Game1.Instance.NetworkManager.StartClient(ip, port);
                    Log("Joining " + ip + ":" + port);
                }
                else if (cmd == "exit" || cmd == "quit")
                {
                    Game1.Instance.Exit();
                }
                else if (cmd == "clear")
                {
                    _log.Clear();
                }
            }
            catch (Exception ex)
            {
                Log("Err: " + ex.Message);
            }
        }
    }
}