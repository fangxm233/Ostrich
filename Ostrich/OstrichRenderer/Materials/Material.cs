﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstrichRenderer.Primitives;
using OstrichRenderer.Rendering;
using OstrichRenderer.RenderMath;
using Random = OstrichRenderer.RenderMath.Random;

namespace OstrichRenderer.Materials
{
    public abstract class Material
    {
        public abstract bool Scatter(Ray rayIn, HitRecord record, ref Color32 attenuation, ref Ray scattered);

        public virtual Color32 Emitted(double u, double v, Vector2 p) => new Color32(0, 0, 0);

        ///获取反射射线
        public static Vector2 Reflect(Vector2 vin, Vector2 normal) => vin - 2 * Vector2.Dot(vin, normal) * normal;

        ///获取折射射线
        public static bool Refract(Vector2 vin, Vector2 normal, double niNo, ref Vector2 refracted)
        {
            Vector2 uvin = vin.Normalize();
            double dt = Vector2.Dot(uvin, normal);
            double discrimination = 1 - niNo * niNo * (1 - dt * dt);
            if (discrimination > 0)
            {
                refracted = niNo * (uvin - normal * dt) - normal * Math.Sqrt(discrimination);
                return true;
            }

            return false;
        }
    }

    public class Light : Material
    {
        /// 强度
        public double Intensity;
        public Color32 Color;

        public Light(Color32 color, double intensity)
        {
            Color = color;
            Intensity = intensity;
        }

        public override bool Scatter(Ray rayIn, HitRecord record, ref Color32 attenuation, ref Ray scattered)
        {
            attenuation = Color * Intensity;
            if (record.T == 0) return false;
            scattered = new Ray(record.P + 0.0001 * record.Normal, Reflect(record.P - rayIn.Origin, record.Normal));
            return true;
        }
    }
}