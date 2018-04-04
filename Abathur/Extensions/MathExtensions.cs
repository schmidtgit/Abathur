using Abathur.Model;
using NydusNetwork.API.Protocol;
using System;
using System.Collections.Generic;

namespace Abathur.Extensions {
    public static class MathExtensions {
        public static Point2D ConvertTo2D(this Point point) => new Point2D { X = point.X,Y = point.Y };

        public static T GetClosest<T>(this IPosition point,IEnumerable<T> enumerable) where T : IPosition
            => GetClosest(point.Point, enumerable);
        public static T GetClosest<T>(this Point2D point,IEnumerable<T> enumerable) where T : IPosition {
            IPosition result = null;
            double minValue = double.MaxValue; double value;
            foreach(var position in enumerable) {
                value = FastDistance(point,position.Point);
                if(value < minValue) {
                    minValue = value;
                    result = position;
                }
            }
            return (T)result;
        }

        public static double FastDistance(this Point2D point,float x, float y) {
            var deltaX = point.X - x;
            var deltaY = point.Y - y;
            return deltaX * deltaX + deltaY * deltaY;
        }
        public static double FastDistance(this Point2D point,Point2D otherPoint)
            => point.FastDistance(otherPoint.X,otherPoint.Y);

        public static double EuclidianDistance(this IUnit unit1,IUnit unit2) => Math.Sqrt(Math.Pow(unit1.Pos.X - unit2.Pos.X,2) + Math.Pow(unit1.Pos.Y - unit2.Pos.Y,2));
        public static double EuclidianDistance(this Point2D p1,IUnit unit) => Math.Sqrt(Math.Pow(p1.X - unit.Pos.X,2) + Math.Pow(p1.Y - unit.Pos.Y,2));
        public static double EuclidianDistance(this Point p1,float x,float y) => Math.Sqrt(Math.Pow(p1.X - x,2) + Math.Pow(p1.Y - y,2));
        public static double EuclidianDistance(this Point p1,Point p2) => Math.Sqrt(Math.Pow(p1.X - p2.X,2) + Math.Pow(p1.Y - p2.Y,2));
        public static double EuclidianDistance(this Point2D p1,Point2D p2) => Math.Sqrt(Math.Pow(p1.X - p2.X,2) + Math.Pow(p1.Y - p2.Y,2));
        public static double EuclidianDistance(this Point2D p1,float x,float y) => Math.Sqrt(Math.Pow(p1.X - x,2) + Math.Pow(p1.Y - y,2));
    }
}
