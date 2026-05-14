using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;

namespace SurviveTheSwarm
{
    public enum EstadoJogo { Menu, AJogar, FimDeJogo }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private EstadoJogo _estadoAtual = EstadoJogo.Menu;
        private SpriteFont _fonte;

        private Texture2D _texturaJogador, _texturaFundo, _texturaInimigo, _texturaMeteoro, _texturaTiro;

        private Vector2 _posicaoJogador;
        private float _velocidadeJogador = 300f;
        private float _rotacaoJogador = 0f;
        private int _vidas = 3;

        private List<Inimigo> _inimigos = new List<Inimigo>();
        private List<Meteoro> _meteoros = new List<Meteoro>();
        private List<Tiro> _tiros = new List<Tiro>();

        private SoundEffect _somTiro;
        private Song _musicaFundo;
        private int _pontuacao = 0;

        private Random _geradorAleatorio = new Random();
        private Matrix _camara;
        private MouseState _estadoRatoAnterior;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        private void ComecarNovoJogo()
        {
            _posicaoJogador = Vector2.Zero;
            _pontuacao = 0;
            _vidas = 3;
            _tiros.Clear();
            _inimigos.Clear();
            _meteoros.Clear();

            for (int i = 0; i < 4; i++) SpawnInimigo();
        }

        private void SpawnInimigo()
        {
            float x = _geradorAleatorio.Next(-1000, 1000);
            float y = _geradorAleatorio.Next(-1000, 1000);

            if (Math.Abs(x) < 400) x = 500;
            if (Math.Abs(y) < 400) y = 500;

            _inimigos.Add(new Inimigo { Posicao = new Vector2(_posicaoJogador.X + x, _posicaoJogador.Y + y) });
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texturaJogador = Content.Load<Texture2D>("playerShip2_blue");
            _texturaFundo = Content.Load<Texture2D>("black");
            _texturaInimigo = Content.Load<Texture2D>("enemyBlack5");
            _texturaTiro = Content.Load<Texture2D>("laserBlue15");
            _texturaMeteoro = Content.Load<Texture2D>("meteorBrown_big1");
            _somTiro = Content.Load<SoundEffect>("laser");
            _fonte = Content.Load<SpriteFont>("Fonte");

            _musicaFundo = Content.Load<Song>("musica");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.Play(_musicaFundo);
        }

        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_estadoAtual != EstadoJogo.AJogar)
            {
                if (kstate.IsKeyDown(Keys.Escape)) Exit();
                if (kstate.IsKeyDown(Keys.Enter)) { ComecarNovoJogo(); _estadoAtual = EstadoJogo.AJogar; }
                return;
            }

            _camara = Matrix.CreateTranslation(-_posicaoJogador.X + (GraphicsDevice.Viewport.Width / 2), -_posicaoJogador.Y + (GraphicsDevice.Viewport.Height / 2), 0);

            // 1. LIMPAR METEOROS LONGE
            for (int i = _meteoros.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(_posicaoJogador, _meteoros[i].Posicao) > 2000)
                {
                    _meteoros.RemoveAt(i);
                }
            }

            // 2. CRIAR METEOROS COM DIREÇÃO E ROTAÇÃO ALEATÓRIA
            while (_meteoros.Count < 20)
            {
                float x = _posicaoJogador.X + _geradorAleatorio.Next(-1500, 1500);
                float y = _posicaoJogador.Y + _geradorAleatorio.Next(-1500, 1500);

                if (Vector2.Distance(_posicaoJogador, new Vector2(x, y)) > 800)
                {
                    // Gera um ângulo aleatório (0 a 360 graus) para a direção em que o meteoro vai flutuar
                    float anguloDirecao = (float)_geradorAleatorio.NextDouble() * MathHelper.TwoPi;
                    Vector2 direcao = new Vector2((float)Math.Cos(anguloDirecao), (float)Math.Sin(anguloDirecao));

                    // Gera uma velocidade de rotação aleatória (para uns rodarem rápido, outros devagar, para a esquerda ou direita)
                    float velocidadeRot = (float)(_geradorAleatorio.NextDouble() - 0.5) * 2f;

                    _meteoros.Add(new Meteoro
                    {
                        Posicao = new Vector2(x, y),
                        Direcao = direcao,
                        Rotacao = (float)_geradorAleatorio.NextDouble() * MathHelper.TwoPi,
                        VelocidadeRotacao = velocidadeRot
                    });
                }
            }

            // 3. MOVER OS METEOROS (A Magia do Espaço!)
            foreach (var m in _meteoros)
            {
                m.Posicao += m.Direcao * m.Velocidade * deltaTime; // Anda devagarinho
                m.Rotacao += m.VelocidadeRotacao * deltaTime;      // Roda sobre si próprio
            }

            // Movimento do Jogador e Colisões com Meteoros
            Vector2 posAnterior = _posicaoJogador;
            if (kstate.IsKeyDown(Keys.W)) _posicaoJogador.Y -= _velocidadeJogador * deltaTime;
            if (kstate.IsKeyDown(Keys.S)) _posicaoJogador.Y += _velocidadeJogador * deltaTime;
            if (kstate.IsKeyDown(Keys.A)) _posicaoJogador.X -= _velocidadeJogador * deltaTime;
            if (kstate.IsKeyDown(Keys.D)) _posicaoJogador.X += _velocidadeJogador * deltaTime;

            Rectangle retPlayer = new Rectangle((int)_posicaoJogador.X - 25, (int)_posicaoJogador.Y - 25, 50, 50);
            foreach (var m in _meteoros) { if (retPlayer.Intersects(m.Retangulo)) _posicaoJogador = posAnterior; }

            // Rotação e Disparo do Jogador
            Vector2 ratoMundo = Vector2.Transform(mouseState.Position.ToVector2(), Matrix.Invert(_camara));
            Vector2 dirRato = ratoMundo - _posicaoJogador;
            _rotacaoJogador = (float)Math.Atan2(dirRato.Y, dirRato.X) + MathHelper.PiOver2;

            if (mouseState.LeftButton == ButtonState.Pressed && _estadoRatoAnterior.LeftButton == ButtonState.Released)
            {
                if (dirRato != Vector2.Zero) dirRato.Normalize();
                _tiros.Add(new Tiro { Posicao = _posicaoJogador, Direcao = dirRato });
                _somTiro.Play();
            }

            // Lógica dos Inimigos
            float velocidadeInimigos = 120f + (_pontuacao / 1000) * 40f;

            for (int i = _inimigos.Count - 1; i >= 0; i--)
            {
                Inimigo inimigo = _inimigos[i];
                Vector2 dirParaPlayer = _posicaoJogador - inimigo.Posicao;
                inimigo.Rotacao = (float)Math.Atan2(dirParaPlayer.Y, dirParaPlayer.X) + MathHelper.PiOver2;

                if (dirParaPlayer != Vector2.Zero)
                {
                    dirParaPlayer.Normalize();
                    inimigo.Posicao += dirParaPlayer * velocidadeInimigos * deltaTime;
                }

                if (new Rectangle((int)inimigo.Posicao.X - 25, (int)inimigo.Posicao.Y - 25, 50, 50).Intersects(retPlayer))
                {
                    _vidas--;
                    _inimigos.RemoveAt(i);
                    SpawnInimigo();

                    if (_vidas <= 0) _estadoAtual = EstadoJogo.FimDeJogo;
                }
            }

            // Lógica dos Tiros
            for (int i = _tiros.Count - 1; i >= 0; i--)
            {
                _tiros[i].Posicao += _tiros[i].Direcao * 800f * deltaTime;
                Rectangle retTiro = new Rectangle((int)_tiros[i].Posicao.X - 5, (int)_tiros[i].Posicao.Y - 5, 10, 10);

                foreach (var m in _meteoros) { if (retTiro.Intersects(m.Retangulo)) { _tiros.RemoveAt(i); break; } }
                if (i >= _tiros.Count) continue;

                for (int j = _inimigos.Count - 1; j >= 0; j--)
                {
                    if (retTiro.Intersects(new Rectangle((int)_inimigos[j].Posicao.X - 25, (int)_inimigos[j].Posicao.Y - 25, 50, 50)))
                    {
                        _tiros.RemoveAt(i); _inimigos.RemoveAt(j); _pontuacao += 100; SpawnInimigo(); break;
                    }
                }
            }

            _estadoRatoAnterior = mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: _camara, samplerState: SamplerState.LinearWrap);

            int larguraEcran = GraphicsDevice.Viewport.Width * 2;
            int alturaEcran = GraphicsDevice.Viewport.Height * 2;

            Rectangle destinoFundo = new Rectangle((int)_posicaoJogador.X - larguraEcran / 2, (int)_posicaoJogador.Y - alturaEcran / 2, larguraEcran, alturaEcran);
            Rectangle origemFundo = new Rectangle((int)_posicaoJogador.X, (int)_posicaoJogador.Y, larguraEcran, alturaEcran);

            _spriteBatch.Draw(_texturaFundo, destinoFundo, origemFundo, Color.White);

            // NOVO: A desenhar os meteoros com a sua rotação em vez de "0f"
            foreach (var m in _meteoros) _spriteBatch.Draw(_texturaMeteoro, m.Posicao, null, Color.White, m.Rotacao, new Vector2(_texturaMeteoro.Width / 2, _texturaMeteoro.Height / 2), 1f, SpriteEffects.None, 0f);

            foreach (var t in _tiros) _spriteBatch.Draw(_texturaTiro, new Rectangle((int)t.Posicao.X - 5, (int)t.Posicao.Y - 5, 10, 10), Color.White);
            foreach (var ini in _inimigos) _spriteBatch.Draw(_texturaInimigo, ini.Posicao, null, Color.White, ini.Rotacao, new Vector2(_texturaInimigo.Width / 2, _texturaInimigo.Height / 2), 1f, SpriteEffects.None, 0f);
            _spriteBatch.Draw(_texturaJogador, _posicaoJogador, null, Color.White, _rotacaoJogador, new Vector2(_texturaJogador.Width / 2, _texturaJogador.Height / 2), 1f, SpriteEffects.None, 0f);

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_fonte, "Score: " + _pontuacao, new Vector2(20, 20), Color.Yellow);

            if (_estadoAtual == EstadoJogo.AJogar)
            {
                _spriteBatch.DrawString(_fonte, "Vidas: ", new Vector2(20, 50), Color.White);
                for (int i = 0; i < _vidas; i++)
                {
                    _spriteBatch.Draw(_texturaJogador, new Rectangle(110 + (i * 35), 50, 30, 30), Color.White);
                }
            }

            if (_estadoAtual == EstadoJogo.Menu) _spriteBatch.DrawString(_fonte, "PRESS ENTER TO START", new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, 400), Color.White);
            if (_estadoAtual == EstadoJogo.FimDeJogo) _spriteBatch.DrawString(_fonte, "GAME OVER! PRESS ENTER", new Vector2(GraphicsDevice.Viewport.Width / 2 - 170, 400), Color.Red);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class Tiro { public Vector2 Posicao, Direcao; }
    public class Inimigo { public Vector2 Posicao; public float Rotacao; }

    // A Classe Meteoro agora tem Direção e Rotação!
    public class Meteoro
    {
        public Vector2 Posicao;
        public Vector2 Direcao;
        public float Velocidade = 30f; // Bem devagarinho
        public float Rotacao;
        public float VelocidadeRotacao;
        public Rectangle Retangulo => new Rectangle((int)Posicao.X - 40, (int)Posicao.Y - 40, 80, 80);
    }
}