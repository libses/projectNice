using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Render
{
    public record PlanetsSettings(int count, int minSize, int maxSize, Brush color);
    public class Planets : Renderable<Planets, PlanetsSettings>
    {
        public List<Planet> PlanetsList = new();
        float sumMass;
        float xMass;
        float yMass;
        public float speed;
        public Planets(int width, int height) : base(width, height)
        {
        }

        public override DirectBitmap GetBitmap()
        {
            var bmp = new DirectBitmap(Width, Height);
            if (PlanetsList.Count == 0)
            {
                var r = new Random();
                PlanetsList = Enumerable
                    .Range(0, Settings.count - 1)
                    .Select(x => new Planet(
                        r.Next(0, Width),
                        r.Next(0, Height),
                        r.Next(Settings.minSize, Settings.maxSize),
                        0, 0))
                    .ToList();

                sumMass = PlanetsList.Sum(x => x.size);

                xMass = Width / 2;
                yMass = Height / 2;

                var xMassD = PlanetsList.Sum(x => x.Position.X * x.size) / sumMass - xMass;
                var yMassD = PlanetsList.Sum(y => y.Position.Y * y.size) / sumMass - yMass;

                PlanetsList = PlanetsList.Select(x => new Planet(x.Position.X - xMassD, x.Position.Y - yMassD, x.size, x.Speed.X, x.Speed.Y)).ToList();
                PlanetsList.Add(new Planet(xMass, yMass, Settings.maxSize, 0, 0));
            }
            
            ApplyGravity();
            var g = Graphics.FromImage(bmp.Bitmap);
            foreach (var planet in PlanetsList)
            {
                g.FillEllipse(Settings.color, planet.Position.X - planet.size / 2, planet.Position.Y - planet.size / 2, planet.size, planet.size);
            }

            return bmp;
        }

        public void ApplyGravity()
        {
            foreach (var planet in PlanetsList)
            {
                planet.Position += planet.Speed * speed;
                foreach (var other in PlanetsList)
                {
                    if (planet == other) continue;
                    var r21 = planet.Position - other.Position;
                    var r21length = r21.Length();
                    var scalarF = -(planet.size * other.size) / (r21length * r21length + planet.size);
                    var r21cup = r21 / (r21length + planet.size);
                    var vectorF = scalarF * r21cup;
                    planet.Speed = planet.Speed + (1.3f + speed) * vectorF / (planet.size);
                }

                if (planet.Position.X < 0)
                {
                    planet.Position.X = 1;
                    planet.Speed.X = -0.5f * planet.Speed.X;
                }

                if (planet.Position.X >= Width)
                {
                    planet.Position.X = Width - 1;
                    planet.Speed.X = -0.5f * planet.Speed.X;
                }

                if (planet.Position.Y < 0)
                {
                    planet.Position.Y = 1;
                    planet.Speed.Y = -0.5f * planet.Speed.Y;
                }

                if (planet.Position.Y >= Height)
                {
                    planet.Position.Y = Height - 1;
                    planet.Speed.Y = -0.5f * planet.Speed.Y;
                }
            }
        }
    }

    public class Planet
    {
        public Vector2 Position;
        public Vector2 Speed;
        public float size;

        public Planet(float x, float y, float r, float speedX, float speedY)
        {
            Position = new Vector2(x, y);
            Speed = new Vector2(0, 0);
            size = r;
        }
    }
}
