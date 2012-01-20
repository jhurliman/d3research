#region License
/*
MIT License
Copyright Â© 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace libdiablo3
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4f : IEquatable<Matrix4f>
    {
        #region Public Fields

        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        #endregion Public Fields


        #region Static Properties

        private static Matrix4f identity = new Matrix4f(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
        public static Matrix4f Identity
        {
            get { return identity; }
        }

        #endregion Static Properties


        #region Public Properties

        public Vector3f Backward
        {
            get { return new Vector3f(this.M31, this.M32, this.M33); }
            set
            {
                this.M31 = value.X;
                this.M32 = value.Y;
                this.M33 = value.Z;
            }
        }

        public Vector3f Down
        {
            get { return new Vector3f(-this.M21, -this.M22, -this.M23); }
            set
            {
                this.M21 = -value.X;
                this.M22 = -value.Y;
                this.M23 = -value.Z;
            }
        }

        public Vector3f Forward
        {
            get { return new Vector3f(-this.M31, -this.M32, -this.M33); }
            set
            {
                this.M31 = -value.X;
                this.M32 = -value.Y;
                this.M33 = -value.Z;
            }
        }

        public Vector3f Left
        {
            get { return new Vector3f(-this.M11, -this.M12, -this.M13); }
            set
            {
                this.M11 = -value.X;
                this.M12 = -value.Y;
                this.M13 = -value.Z;
            }
        }

        public Vector3f Right
        {
            get { return new Vector3f(this.M11, this.M12, this.M13); }
            set
            {
                this.M11 = value.X;
                this.M12 = value.Y;
                this.M13 = value.Z;
            }
        }

        public Vector3f Translation
        {
            get { return new Vector3f(this.M41, this.M42, this.M43); }
            set
            {
                this.M41 = value.X;
                this.M42 = value.Y;
                this.M43 = value.Z;
            }
        }

        public Vector3f Up
        {
            get { return new Vector3f(this.M21, this.M22, this.M23); }
            set
            {
                this.M21 = value.X;
                this.M22 = value.Y;
                this.M23 = value.Z;
            }
        }

        #endregion Public Properties


        #region Constructors
        /// <summary>
        /// Constructor for 4x4 Matrix
        /// </summary>
        /// <param name="m11">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m12">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m13">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m14">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m21">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m22">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m23">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m24">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m31">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m32">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m33">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m34">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m41">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m42">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m43">
        /// A <see cref="System.Single"/>
        /// </param>
        /// <param name="m44">
        /// A <see cref="System.Single"/>
        /// </param>
        public Matrix4f(float m11, float m12, float m13, float m14, float m21, float m22, float m23, float m24,
                      float m31, float m32, float m33, float m34, float m41, float m42, float m43, float m44)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
        }

        #endregion Constructors

        #region Public Static Methods

        public static Matrix4f CreateWorld(Vector3f position, Vector3f forward, Vector3f up)
        {
            Matrix4f ret;
            CreateWorld(ref position, ref forward, ref up, out ret);
            return ret;
        }

        public static void CreateWorld(ref Vector3f position, ref Vector3f forward, ref Vector3f up, out Matrix4f result)
        {
            Vector3f x, y, z;
            Vector3f.Normalize(ref forward, out z);
            Vector3f.Cross(ref forward, ref up, out x);
            Vector3f.Cross(ref x, ref forward, out y);
            x.Normalize();
            y.Normalize();

            result = new Matrix4f();
            result.Right = x;
            result.Up = y;
            result.Forward = z;
            result.Translation = position;
            result.M44 = 1f;
        }

        public static Matrix4f CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Matrix4f matrix;
            Quaternion quaternion;
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            CreateFromQuaternion(ref quaternion, out matrix);
            return matrix;
        }

        public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Matrix4f result)
        {
            Quaternion quaternion;
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            CreateFromQuaternion(ref quaternion, out result);
        }

        public static void Transform(ref Matrix4f value, ref Quaternion rotation, out Matrix4f result)
        {
            Matrix4f matrix = CreateFromQuaternion(rotation);
            Matrix4f.Multiply(ref value, ref matrix, out result);
        }

        public static Matrix4f Transform(Matrix4f value, Quaternion rotation)
        {
            Matrix4f ret;
            Transform(ref value, ref rotation, out ret);
            return ret;
        }

        public bool Decompose(out Vector3f scale, out Quaternion rotation, out Vector3f translation)
        {
            translation.X = this.M41;
            translation.Y = this.M42;
            translation.Z = this.M43;
            float xs, ys, zs;

            if (Math.Sign(M11 * M12 * M13 * M14) < 0)
                xs = -1f;
            else
                xs = 1f;

            if (Math.Sign(M21 * M22 * M23 * M24) < 0)
                ys = -1f;
            else
                ys = 1f;

            if (Math.Sign(M31 * M32 * M33 * M34) < 0)
                zs = -1f;
            else
                zs = 1f;

            scale.X = xs * (float)Math.Sqrt(this.M11 * this.M11 + this.M12 * this.M12 + this.M13 * this.M13);
            scale.Y = ys * (float)Math.Sqrt(this.M21 * this.M21 + this.M22 * this.M22 + this.M23 * this.M23);
            scale.Z = zs * (float)Math.Sqrt(this.M31 * this.M31 + this.M32 * this.M32 + this.M33 * this.M33);

            if (scale.X == 0.0 || scale.Y == 0.0 || scale.Z == 0.0)
            {
                rotation = Quaternion.Identity;
                return false;
            }

            Matrix4f m1 = new Matrix4f(this.M11 / scale.X, M12 / scale.X, M13 / scale.X, 0,
                this.M21 / scale.Y, M22 / scale.Y, M23 / scale.Y, 0,
                this.M31 / scale.Z, M32 / scale.Z, M33 / scale.Z, 0,
                0, 0, 0, 1);

            rotation = Quaternion.CreateFromRotationMatrix(m1);
            return true;
        }

        /// <summary>
        /// Adds second matrix to the first.
        /// </summary>
        /// <param name="matrix1">
        /// A <see cref="Matrix4f"/>
        /// </param>
        /// <param name="matrix2">
        /// A <see cref="Matrix4f"/>
        /// </param>
        /// <returns>
        /// A <see cref="Matrix4f"/>
        /// </returns>
        public static Matrix4f Add(Matrix4f matrix1, Matrix4f matrix2)
        {
            matrix1.M11 += matrix2.M11;
            matrix1.M12 += matrix2.M12;
            matrix1.M13 += matrix2.M13;
            matrix1.M14 += matrix2.M14;
            matrix1.M21 += matrix2.M21;
            matrix1.M22 += matrix2.M22;
            matrix1.M23 += matrix2.M23;
            matrix1.M24 += matrix2.M24;
            matrix1.M31 += matrix2.M31;
            matrix1.M32 += matrix2.M32;
            matrix1.M33 += matrix2.M33;
            matrix1.M34 += matrix2.M34;
            matrix1.M41 += matrix2.M41;
            matrix1.M42 += matrix2.M42;
            matrix1.M43 += matrix2.M43;
            matrix1.M44 += matrix2.M44;
            return matrix1;
        }


        /// <summary>
        /// Adds two Matrix and save to the result Matrix
        /// </summary>
        /// <param name="matrix1">
        /// A <see cref="Matrix4f"/>
        /// </param>
        /// <param name="matrix2">
        /// A <see cref="Matrix4f"/>
        /// </param>
        /// <param name="result">
        /// A <see cref="Matrix4f"/>
        /// </param>
        public static void Add(ref Matrix4f matrix1, ref Matrix4f matrix2, out Matrix4f result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M14 = matrix1.M14 + matrix2.M14;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M24 = matrix1.M24 + matrix2.M24;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            result.M34 = matrix1.M34 + matrix2.M34;
            result.M41 = matrix1.M41 + matrix2.M41;
            result.M42 = matrix1.M42 + matrix2.M42;
            result.M43 = matrix1.M43 + matrix2.M43;
            result.M44 = matrix1.M44 + matrix2.M44;
        }


        public static Matrix4f CreateBillboard(Vector3f objectPosition, Vector3f cameraPosition,
            Vector3f cameraUpVector, Nullable<Vector3f> cameraForwardVector)
        {
            Matrix4f ret;
            CreateBillboard(ref objectPosition, ref cameraPosition, ref cameraUpVector, cameraForwardVector, out ret);
            return ret;
        }

        public static void CreateBillboard(ref Vector3f objectPosition, ref Vector3f cameraPosition,
            ref Vector3f cameraUpVector, Vector3f? cameraForwardVector, out Matrix4f result)
        {
            Vector3f translation = objectPosition - cameraPosition;
            Vector3f backwards, right, up;
            Vector3f.Normalize(ref translation, out backwards);
            Vector3f.Normalize(ref cameraUpVector, out up);
            Vector3f.Cross(ref backwards, ref up, out right);
            Vector3f.Cross(ref backwards, ref right, out up);
            result = Matrix4f.Identity;
            result.Backward = backwards;
            result.Right = right;
            result.Up = up;
            result.Translation = translation;
        }

        public static Matrix4f CreateConstrainedBillboard(Vector3f objectPosition, Vector3f cameraPosition,
            Vector3f rotateAxis, Nullable<Vector3f> cameraForwardVector, Nullable<Vector3f> objectForwardVector)
        {
            throw new NotImplementedException();
        }


        public static void CreateConstrainedBillboard(ref Vector3f objectPosition, ref Vector3f cameraPosition,
            ref Vector3f rotateAxis, Vector3f? cameraForwardVector, Vector3f? objectForwardVector, out Matrix4f result)
        {
            throw new NotImplementedException();
        }


        public static Matrix4f CreateFromAxisAngle(Vector3f axis, float angle)
        {
            throw new NotImplementedException();
        }


        public static void CreateFromAxisAngle(ref Vector3f axis, float angle, out Matrix4f result)
        {
            throw new NotImplementedException();
        }


        public static Matrix4f CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix4f ret;
            CreateFromQuaternion(ref quaternion, out ret);
            return ret;
        }


        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M11 = 1 - 2 * (quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            result.M12 = 2 * (quaternion.X * quaternion.Y + quaternion.W * quaternion.Z);
            result.M13 = 2 * (quaternion.X * quaternion.Z - quaternion.W * quaternion.Y);
            result.M21 = 2 * (quaternion.X * quaternion.Y - quaternion.W * quaternion.Z);
            result.M22 = 1 - 2 * (quaternion.X * quaternion.X + quaternion.Z * quaternion.Z);
            result.M23 = 2 * (quaternion.Y * quaternion.Z + quaternion.W * quaternion.X);
            result.M31 = 2 * (quaternion.X * quaternion.Z + quaternion.W * quaternion.Y);
            result.M32 = 2 * (quaternion.Y * quaternion.Z - quaternion.W * quaternion.X);
            result.M33 = 1 - 2 * (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y);
        }


        public static Matrix4f CreateLookAt(Vector3f cameraPosition, Vector3f cameraTarget, Vector3f cameraUpVector)
        {
            Matrix4f ret;
            CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out ret);
            return ret;
        }


        public static void CreateLookAt(ref Vector3f cameraPosition, ref Vector3f cameraTarget, ref Vector3f cameraUpVector, out Matrix4f result)
        {
            // http://msdn.microsoft.com/en-us/library/bb205343(v=VS.85).aspx

            Vector3f vz = Vector3f.Normalize(cameraPosition - cameraTarget);
            Vector3f vx = Vector3f.Normalize(Vector3f.Cross(cameraUpVector, vz));
            Vector3f vy = Vector3f.Cross(vz, vx);
            result = Matrix4f.Identity;
            result.M11 = vx.X;
            result.M12 = vy.X;
            result.M13 = vz.X;
            result.M21 = vx.Y;
            result.M22 = vy.Y;
            result.M23 = vz.Y;
            result.M31 = vx.Z;
            result.M32 = vy.Z;
            result.M33 = vz.Z;
            result.M41 = -Vector3f.Dot(vx, cameraPosition);
            result.M42 = -Vector3f.Dot(vy, cameraPosition);
            result.M43 = -Vector3f.Dot(vz, cameraPosition);
        }

        public static Matrix4f CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
        {
            Matrix4f ret;
            CreateOrthographic(width, height, zNearPlane, zFarPlane, out ret);
            return ret;
        }


        public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix4f result)
        {
            result.M11 = 2 / width;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = 2 / height;
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1 / (zNearPlane - zFarPlane);
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = 1;
        }


        public static Matrix4f CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
            Matrix4f ret;
            CreateOrthographicOffCenter(left, right, bottom, top, zNearPlane, zFarPlane, out ret);
            return ret;
        }


        public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top,
            float zNearPlane, float zFarPlane, out Matrix4f result)
        {
            result.M11 = 2 / (right - left);
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;
            result.M21 = 0;
            result.M22 = 2 / (top - bottom);
            result.M23 = 0;
            result.M24 = 0;
            result.M31 = 0;
            result.M32 = 0;
            result.M33 = 1 / (zNearPlane - zFarPlane);
            result.M34 = 0;
            result.M41 = (left + right) / (left - right);
            result.M42 = (bottom + top) / (bottom - top);
            result.M43 = zNearPlane / (zNearPlane - zFarPlane);
            result.M44 = 1;
        }


        public static Matrix4f CreatePerspective(float width, float height, float zNearPlane, float zFarPlane)
        {
            throw new NotImplementedException();
        }


        public static void CreatePerspective(float width, float height, float zNearPlane, float zFarPlane, out Matrix4f result)
        {
            throw new NotImplementedException();
        }


        public static Matrix4f CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            Matrix4f ret;
            CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance, out ret);
            return ret;
        }


        public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix4f result)
        {
            // http://msdn.microsoft.com/en-us/library/bb205351(v=VS.85).aspx
            // http://msdn.microsoft.com/en-us/library/bb195665.aspx

            result = new Matrix4f(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            if (fieldOfView < 0 || fieldOfView > 3.14159262f)
                throw new ArgumentOutOfRangeException("fieldOfView", "fieldOfView takes a value between 0 and Pi (180 degrees) in radians.");

            if (nearPlaneDistance <= 0.0f)
                throw new ArgumentOutOfRangeException("nearPlaneDistance", "You should specify positive value for nearPlaneDistance.");

            if (farPlaneDistance <= 0.0f)
                throw new ArgumentOutOfRangeException("farPlaneDistance", "You should specify positive value for farPlaneDistance.");

            if (farPlaneDistance <= nearPlaneDistance)
                throw new ArgumentOutOfRangeException("nearPlaneDistance", "Near plane distance is larger than Far plane distance. Near plane distance must be smaller than Far plane distance.");

            float yscale = (float)1 / (float)Math.Tan(fieldOfView / 2);
            float xscale = yscale / aspectRatio;

            result.M11 = xscale;
            result.M22 = yscale;
            result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M34 = -1;
            result.M43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        }


        public static Matrix4f CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
        {
            throw new NotImplementedException();
        }


        public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance, out Matrix4f result)
        {
            throw new NotImplementedException();
        }


        public static Matrix4f CreateRotationX(float radians)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M22 = (float)Math.Cos(radians);
            returnMatrix.M23 = (float)Math.Sin(radians);
            returnMatrix.M32 = -returnMatrix.M23;
            returnMatrix.M33 = returnMatrix.M22;

            return returnMatrix;
        }


        public static void CreateRotationX(float radians, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M22 = (float)Math.Cos(radians);
            result.M23 = (float)Math.Sin(radians);
            result.M32 = -result.M23;
            result.M33 = result.M22;
        }


        public static Matrix4f CreateRotationY(float radians)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M11 = (float)Math.Cos(radians);
            returnMatrix.M13 = (float)Math.Sin(radians);
            returnMatrix.M31 = -returnMatrix.M13;
            returnMatrix.M33 = returnMatrix.M11;

            return returnMatrix;
        }


        public static void CreateRotationY(float radians, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M11 = (float)Math.Cos(radians);
            result.M13 = (float)Math.Sin(radians);
            result.M31 = -result.M13;
            result.M33 = result.M11;
        }


        public static Matrix4f CreateRotationZ(float radians)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M11 = (float)Math.Cos(radians);
            returnMatrix.M12 = (float)Math.Sin(radians);
            returnMatrix.M21 = -returnMatrix.M12;
            returnMatrix.M22 = returnMatrix.M11;

            return returnMatrix;
        }


        public static void CreateRotationZ(float radians, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M11 = (float)Math.Cos(radians);
            result.M12 = (float)Math.Sin(radians);
            result.M21 = -result.M12;
            result.M22 = result.M11;
        }


        public static Matrix4f CreateScale(float scale)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M11 = scale;
            returnMatrix.M22 = scale;
            returnMatrix.M33 = scale;

            return returnMatrix;
        }


        public static void CreateScale(float scale, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M11 = scale;
            result.M22 = scale;
            result.M33 = scale;
        }


        public static Matrix4f CreateScale(float xScale, float yScale, float zScale)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M11 = xScale;
            returnMatrix.M22 = yScale;
            returnMatrix.M33 = zScale;

            return returnMatrix;
        }


        public static void CreateScale(float xScale, float yScale, float zScale, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M11 = xScale;
            result.M22 = yScale;
            result.M33 = zScale;
        }


        public static Matrix4f CreateScale(Vector3f scales)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M11 = scales.X;
            returnMatrix.M22 = scales.Y;
            returnMatrix.M33 = scales.Z;

            return returnMatrix;
        }


        public static void CreateScale(ref Vector3f scales, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M11 = scales.X;
            result.M22 = scales.Y;
            result.M33 = scales.Z;
        }


        public static Matrix4f CreateTranslation(float xPosition, float yPosition, float zPosition)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M41 = xPosition;
            returnMatrix.M42 = yPosition;
            returnMatrix.M43 = zPosition;

            return returnMatrix;
        }


        public static void CreateTranslation(float xPosition, float yPosition, float zPosition, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M41 = xPosition;
            result.M42 = yPosition;
            result.M43 = zPosition;
        }


        public static Matrix4f CreateTranslation(Vector3f position)
        {
            Matrix4f returnMatrix = Matrix4f.Identity;

            returnMatrix.M41 = position.X;
            returnMatrix.M42 = position.Y;
            returnMatrix.M43 = position.Z;

            return returnMatrix;
        }


        public static void CreateTranslation(ref Vector3f position, out Matrix4f result)
        {
            result = Matrix4f.Identity;

            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
        }

        public static Matrix4f Divide(Matrix4f matrix1, Matrix4f matrix2)
        {
            Matrix4f inverse = Matrix4f.Invert(matrix2), result;

            result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
            result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
            result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
            result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;

            result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
            result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
            result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
            result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;

            result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
            result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
            result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
            result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;

            result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
            result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
            result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
            result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;

            return result;
        }


        public static void Divide(ref Matrix4f matrix1, ref Matrix4f matrix2, out Matrix4f result)
        {
            Matrix4f inverse = Matrix4f.Invert(matrix2);
            result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
            result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
            result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
            result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;

            result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
            result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
            result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
            result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;

            result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
            result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
            result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
            result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;

            result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
            result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
            result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
            result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;
        }


        public static Matrix4f Divide(Matrix4f matrix1, float divider)
        {
            float inverseDivider = 1.0f / divider;

            matrix1.M11 = matrix1.M11 * inverseDivider;
            matrix1.M12 = matrix1.M12 * inverseDivider;
            matrix1.M13 = matrix1.M13 * inverseDivider;
            matrix1.M14 = matrix1.M14 * inverseDivider;
            matrix1.M21 = matrix1.M21 * inverseDivider;
            matrix1.M22 = matrix1.M22 * inverseDivider;
            matrix1.M23 = matrix1.M23 * inverseDivider;
            matrix1.M24 = matrix1.M24 * inverseDivider;
            matrix1.M31 = matrix1.M31 * inverseDivider;
            matrix1.M32 = matrix1.M32 * inverseDivider;
            matrix1.M33 = matrix1.M33 * inverseDivider;
            matrix1.M34 = matrix1.M34 * inverseDivider;
            matrix1.M41 = matrix1.M41 * inverseDivider;
            matrix1.M42 = matrix1.M42 * inverseDivider;
            matrix1.M43 = matrix1.M43 * inverseDivider;
            matrix1.M44 = matrix1.M44 * inverseDivider;

            return matrix1;
        }


        public static void Divide(ref Matrix4f matrix1, float divider, out Matrix4f result)
        {
            float inverseDivider = 1.0f / divider;
            result.M11 = matrix1.M11 * inverseDivider;
            result.M12 = matrix1.M12 * inverseDivider;
            result.M13 = matrix1.M13 * inverseDivider;
            result.M14 = matrix1.M14 * inverseDivider;
            result.M21 = matrix1.M21 * inverseDivider;
            result.M22 = matrix1.M22 * inverseDivider;
            result.M23 = matrix1.M23 * inverseDivider;
            result.M24 = matrix1.M24 * inverseDivider;
            result.M31 = matrix1.M31 * inverseDivider;
            result.M32 = matrix1.M32 * inverseDivider;
            result.M33 = matrix1.M33 * inverseDivider;
            result.M34 = matrix1.M34 * inverseDivider;
            result.M41 = matrix1.M41 * inverseDivider;
            result.M42 = matrix1.M42 * inverseDivider;
            result.M43 = matrix1.M43 * inverseDivider;
            result.M44 = matrix1.M44 * inverseDivider;
        }

        public static Matrix4f Invert(Matrix4f matrix)
        {
            Invert(ref matrix, out matrix);
            return matrix;
        }


        public static void Invert(ref Matrix4f matrix, out Matrix4f result)
        {
            //
            // Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
            // 
            // 1. Calculate the 2x2 determinants needed and the 4x4 determinant based on the 2x2 determinants 
            // 2. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
            // 3. Divide adjugate matrix with the determinant to find the inverse

            float det1 = matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21;
            float det2 = matrix.M11 * matrix.M23 - matrix.M13 * matrix.M21;
            float det3 = matrix.M11 * matrix.M24 - matrix.M14 * matrix.M21;
            float det4 = matrix.M12 * matrix.M23 - matrix.M13 * matrix.M22;
            float det5 = matrix.M12 * matrix.M24 - matrix.M14 * matrix.M22;
            float det6 = matrix.M13 * matrix.M24 - matrix.M14 * matrix.M23;
            float det7 = matrix.M31 * matrix.M42 - matrix.M32 * matrix.M41;
            float det8 = matrix.M31 * matrix.M43 - matrix.M33 * matrix.M41;
            float det9 = matrix.M31 * matrix.M44 - matrix.M34 * matrix.M41;
            float det10 = matrix.M32 * matrix.M43 - matrix.M33 * matrix.M42;
            float det11 = matrix.M32 * matrix.M44 - matrix.M34 * matrix.M42;
            float det12 = matrix.M33 * matrix.M44 - matrix.M34 * matrix.M43;

            float detMatrix = (float)(det1 * det12 - det2 * det11 + det3 * det10 + det4 * det9 - det5 * det8 + det6 * det7);

            float invDetMatrix = 1f / detMatrix;

            Matrix4f ret; // Allow for matrix and result to point to the same structure

            ret.M11 = (matrix.M22 * det12 - matrix.M23 * det11 + matrix.M24 * det10) * invDetMatrix;
            ret.M12 = (-matrix.M12 * det12 + matrix.M13 * det11 - matrix.M14 * det10) * invDetMatrix;
            ret.M13 = (matrix.M42 * det6 - matrix.M43 * det5 + matrix.M44 * det4) * invDetMatrix;
            ret.M14 = (-matrix.M32 * det6 + matrix.M33 * det5 - matrix.M34 * det4) * invDetMatrix;
            ret.M21 = (-matrix.M21 * det12 + matrix.M23 * det9 - matrix.M24 * det8) * invDetMatrix;
            ret.M22 = (matrix.M11 * det12 - matrix.M13 * det9 + matrix.M14 * det8) * invDetMatrix;
            ret.M23 = (-matrix.M41 * det6 + matrix.M43 * det3 - matrix.M44 * det2) * invDetMatrix;
            ret.M24 = (matrix.M31 * det6 - matrix.M33 * det3 + matrix.M34 * det2) * invDetMatrix;
            ret.M31 = (matrix.M21 * det11 - matrix.M22 * det9 + matrix.M24 * det7) * invDetMatrix;
            ret.M32 = (-matrix.M11 * det11 + matrix.M12 * det9 - matrix.M14 * det7) * invDetMatrix;
            ret.M33 = (matrix.M41 * det5 - matrix.M42 * det3 + matrix.M44 * det1) * invDetMatrix;
            ret.M34 = (-matrix.M31 * det5 + matrix.M32 * det3 - matrix.M34 * det1) * invDetMatrix;
            ret.M41 = (-matrix.M21 * det10 + matrix.M22 * det8 - matrix.M23 * det7) * invDetMatrix;
            ret.M42 = (matrix.M11 * det10 - matrix.M12 * det8 + matrix.M13 * det7) * invDetMatrix;
            ret.M43 = (-matrix.M41 * det4 + matrix.M42 * det2 - matrix.M43 * det1) * invDetMatrix;
            ret.M44 = (matrix.M31 * det4 - matrix.M32 * det2 + matrix.M33 * det1) * invDetMatrix;

            result = ret;
        }


        public static Matrix4f Lerp(Matrix4f matrix1, Matrix4f matrix2, float amount)
        {
            throw new NotImplementedException();
        }


        public static void Lerp(ref Matrix4f matrix1, ref Matrix4f matrix2, float amount, out Matrix4f result)
        {
            throw new NotImplementedException();
        }

        public static Matrix4f Multiply(Matrix4f matrix1, Matrix4f matrix2)
        {
            Matrix4f result;

            result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
            result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
            result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
            result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

            result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
            result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
            result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
            result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

            result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
            result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
            result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
            result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

            result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
            result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
            result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
            result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;

            return result;
        }


        public static void Multiply(ref Matrix4f matrix1, ref Matrix4f matrix2, out Matrix4f result)
        {
            result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
            result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
            result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
            result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

            result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
            result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
            result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
            result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

            result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
            result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
            result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
            result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

            result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
            result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
            result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
            result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
        }


        public static Matrix4f Multiply(Matrix4f matrix1, float factor)
        {
            matrix1.M11 *= factor;
            matrix1.M12 *= factor;
            matrix1.M13 *= factor;
            matrix1.M14 *= factor;
            matrix1.M21 *= factor;
            matrix1.M22 *= factor;
            matrix1.M23 *= factor;
            matrix1.M24 *= factor;
            matrix1.M31 *= factor;
            matrix1.M32 *= factor;
            matrix1.M33 *= factor;
            matrix1.M34 *= factor;
            matrix1.M41 *= factor;
            matrix1.M42 *= factor;
            matrix1.M43 *= factor;
            matrix1.M44 *= factor;
            return matrix1;
        }


        public static void Multiply(ref Matrix4f matrix1, float factor, out Matrix4f result)
        {
            result.M11 = matrix1.M11 * factor;
            result.M12 = matrix1.M12 * factor;
            result.M13 = matrix1.M13 * factor;
            result.M14 = matrix1.M14 * factor;
            result.M21 = matrix1.M21 * factor;
            result.M22 = matrix1.M22 * factor;
            result.M23 = matrix1.M23 * factor;
            result.M24 = matrix1.M24 * factor;
            result.M31 = matrix1.M31 * factor;
            result.M32 = matrix1.M32 * factor;
            result.M33 = matrix1.M33 * factor;
            result.M34 = matrix1.M34 * factor;
            result.M41 = matrix1.M41 * factor;
            result.M42 = matrix1.M42 * factor;
            result.M43 = matrix1.M43 * factor;
            result.M44 = matrix1.M44 * factor;
        }


        public static Matrix4f Negate(Matrix4f matrix)
        {
            matrix.M11 = -matrix.M11;
            matrix.M12 = -matrix.M12;
            matrix.M13 = -matrix.M13;
            matrix.M14 = -matrix.M14;
            matrix.M21 = -matrix.M21;
            matrix.M22 = -matrix.M22;
            matrix.M23 = -matrix.M23;
            matrix.M24 = -matrix.M24;
            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M33 = -matrix.M33;
            matrix.M34 = -matrix.M34;
            matrix.M41 = -matrix.M41;
            matrix.M42 = -matrix.M42;
            matrix.M43 = -matrix.M43;
            matrix.M44 = -matrix.M44;
            return matrix;
        }


        public static void Negate(ref Matrix4f matrix, out Matrix4f result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M13 = matrix.M13;
            result.M14 = matrix.M14;
            result.M21 = matrix.M21;
            result.M22 = matrix.M22;
            result.M23 = matrix.M23;
            result.M24 = matrix.M24;
            result.M31 = matrix.M31;
            result.M32 = matrix.M32;
            result.M33 = matrix.M33;
            result.M34 = matrix.M34;
            result.M41 = matrix.M41;
            result.M42 = matrix.M42;
            result.M43 = matrix.M43;
            result.M44 = matrix.M44;
        }

        public static Matrix4f Subtract(Matrix4f matrix1, Matrix4f matrix2)
        {
            matrix1.M11 -= matrix2.M11;
            matrix1.M12 -= matrix2.M12;
            matrix1.M13 -= matrix2.M13;
            matrix1.M14 -= matrix2.M14;
            matrix1.M21 -= matrix2.M21;
            matrix1.M22 -= matrix2.M22;
            matrix1.M23 -= matrix2.M23;
            matrix1.M24 -= matrix2.M24;
            matrix1.M31 -= matrix2.M31;
            matrix1.M32 -= matrix2.M32;
            matrix1.M33 -= matrix2.M33;
            matrix1.M34 -= matrix2.M34;
            matrix1.M41 -= matrix2.M41;
            matrix1.M42 -= matrix2.M42;
            matrix1.M43 -= matrix2.M43;
            matrix1.M44 -= matrix2.M44;
            return matrix1;
        }

        public static void Subtract(ref Matrix4f matrix1, ref Matrix4f matrix2, out Matrix4f result)
        {
            result.M11 = matrix1.M11 - matrix2.M11;
            result.M12 = matrix1.M12 - matrix2.M12;
            result.M13 = matrix1.M13 - matrix2.M13;
            result.M14 = matrix1.M14 - matrix2.M14;
            result.M21 = matrix1.M21 - matrix2.M21;
            result.M22 = matrix1.M22 - matrix2.M22;
            result.M23 = matrix1.M23 - matrix2.M23;
            result.M24 = matrix1.M24 - matrix2.M24;
            result.M31 = matrix1.M31 - matrix2.M31;
            result.M32 = matrix1.M32 - matrix2.M32;
            result.M33 = matrix1.M33 - matrix2.M33;
            result.M34 = matrix1.M34 - matrix2.M34;
            result.M41 = matrix1.M41 - matrix2.M41;
            result.M42 = matrix1.M42 - matrix2.M42;
            result.M43 = matrix1.M43 - matrix2.M43;
            result.M44 = matrix1.M44 - matrix2.M44;
        }

        public static Matrix4f Transpose(Matrix4f matrix)
        {
            Matrix4f result;

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;

            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;

            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;

            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;

            return result;
        }


        public static void Transpose(ref Matrix4f matrix, out Matrix4f result)
        {
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;

            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;

            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;

            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;
        }

        #endregion Public Static Methods

        #region Public Methods

        public float Determinant()
        {
            float minor1, minor2, minor3, minor4, minor5, minor6;

            minor1 = M31 * M42 - M32 * M41;
            minor2 = M31 * M43 - M33 * M41;
            minor3 = M31 * M44 - M34 * M41;
            minor4 = M32 * M43 - M33 * M42;
            minor5 = M32 * M44 - M34 * M42;
            minor6 = M33 * M44 - M34 * M43;

            return M11 * (M22 * minor6 - M23 * minor5 + M24 * minor4) -
                    M12 * (M21 * minor6 - M23 * minor3 + M24 * minor2) +
                    M13 * (M21 * minor5 - M22 * minor3 + M24 * minor1) -
                    M14 * (M21 * minor4 - M22 * minor2 + M23 * minor1);
        }

        public bool Equals(Matrix4f other)
        {
            return (this.M11 == other.M11) && (this.M12 == other.M12) &&
                   (this.M13 == other.M13) && (this.M14 == other.M14) &&
                   (this.M21 == other.M21) && (this.M22 == other.M22) &&
                   (this.M23 == other.M23) && (this.M24 == other.M24) &&
                   (this.M31 == other.M31) && (this.M32 == other.M32) &&
                   (this.M33 == other.M33) && (this.M34 == other.M34) &&
                   (this.M41 == other.M41) && (this.M42 == other.M42) &&
                   (this.M43 == other.M43) && (this.M44 == other.M44);
        }

        #endregion Public Methods

        #region Operators

        public static Matrix4f operator +(Matrix4f matrix1, Matrix4f matrix2)
        {
            matrix1.M11 += matrix2.M11;
            matrix1.M12 += matrix2.M12;
            matrix1.M13 += matrix2.M13;
            matrix1.M14 += matrix2.M14;
            matrix1.M21 += matrix2.M21;
            matrix1.M22 += matrix2.M22;
            matrix1.M23 += matrix2.M23;
            matrix1.M24 += matrix2.M24;
            matrix1.M31 += matrix2.M31;
            matrix1.M32 += matrix2.M32;
            matrix1.M33 += matrix2.M33;
            matrix1.M34 += matrix2.M34;
            matrix1.M41 += matrix2.M41;
            matrix1.M42 += matrix2.M42;
            matrix1.M43 += matrix2.M43;
            matrix1.M44 += matrix2.M44;
            return matrix1;
        }

        public static Matrix4f operator /(Matrix4f matrix1, Matrix4f matrix2)
        {
            Matrix4f inverse = Matrix4f.Invert(matrix2), result;

            result.M11 = matrix1.M11 * inverse.M11 + matrix1.M12 * inverse.M21 + matrix1.M13 * inverse.M31 + matrix1.M14 * inverse.M41;
            result.M12 = matrix1.M11 * inverse.M12 + matrix1.M12 * inverse.M22 + matrix1.M13 * inverse.M32 + matrix1.M14 * inverse.M42;
            result.M13 = matrix1.M11 * inverse.M13 + matrix1.M12 * inverse.M23 + matrix1.M13 * inverse.M33 + matrix1.M14 * inverse.M43;
            result.M14 = matrix1.M11 * inverse.M14 + matrix1.M12 * inverse.M24 + matrix1.M13 * inverse.M34 + matrix1.M14 * inverse.M44;

            result.M21 = matrix1.M21 * inverse.M11 + matrix1.M22 * inverse.M21 + matrix1.M23 * inverse.M31 + matrix1.M24 * inverse.M41;
            result.M22 = matrix1.M21 * inverse.M12 + matrix1.M22 * inverse.M22 + matrix1.M23 * inverse.M32 + matrix1.M24 * inverse.M42;
            result.M23 = matrix1.M21 * inverse.M13 + matrix1.M22 * inverse.M23 + matrix1.M23 * inverse.M33 + matrix1.M24 * inverse.M43;
            result.M24 = matrix1.M21 * inverse.M14 + matrix1.M22 * inverse.M24 + matrix1.M23 * inverse.M34 + matrix1.M24 * inverse.M44;

            result.M31 = matrix1.M31 * inverse.M11 + matrix1.M32 * inverse.M21 + matrix1.M33 * inverse.M31 + matrix1.M34 * inverse.M41;
            result.M32 = matrix1.M31 * inverse.M12 + matrix1.M32 * inverse.M22 + matrix1.M33 * inverse.M32 + matrix1.M34 * inverse.M42;
            result.M33 = matrix1.M31 * inverse.M13 + matrix1.M32 * inverse.M23 + matrix1.M33 * inverse.M33 + matrix1.M34 * inverse.M43;
            result.M34 = matrix1.M31 * inverse.M14 + matrix1.M32 * inverse.M24 + matrix1.M33 * inverse.M34 + matrix1.M34 * inverse.M44;

            result.M41 = matrix1.M41 * inverse.M11 + matrix1.M42 * inverse.M21 + matrix1.M43 * inverse.M31 + matrix1.M44 * inverse.M41;
            result.M42 = matrix1.M41 * inverse.M12 + matrix1.M42 * inverse.M22 + matrix1.M43 * inverse.M32 + matrix1.M44 * inverse.M42;
            result.M43 = matrix1.M41 * inverse.M13 + matrix1.M42 * inverse.M23 + matrix1.M43 * inverse.M33 + matrix1.M44 * inverse.M43;
            result.M44 = matrix1.M41 * inverse.M14 + matrix1.M42 * inverse.M24 + matrix1.M43 * inverse.M34 + matrix1.M44 * inverse.M44;

            return result;
        }

        public static Matrix4f operator /(Matrix4f matrix1, float divider)
        {
            float inverseDivider = 1.0f / divider;

            matrix1.M11 = matrix1.M11 * inverseDivider;
            matrix1.M12 = matrix1.M12 * inverseDivider;
            matrix1.M13 = matrix1.M13 * inverseDivider;
            matrix1.M14 = matrix1.M14 * inverseDivider;
            matrix1.M21 = matrix1.M21 * inverseDivider;
            matrix1.M22 = matrix1.M22 * inverseDivider;
            matrix1.M23 = matrix1.M23 * inverseDivider;
            matrix1.M24 = matrix1.M24 * inverseDivider;
            matrix1.M31 = matrix1.M31 * inverseDivider;
            matrix1.M32 = matrix1.M32 * inverseDivider;
            matrix1.M33 = matrix1.M33 * inverseDivider;
            matrix1.M34 = matrix1.M34 * inverseDivider;
            matrix1.M41 = matrix1.M41 * inverseDivider;
            matrix1.M42 = matrix1.M42 * inverseDivider;
            matrix1.M43 = matrix1.M43 * inverseDivider;
            matrix1.M44 = matrix1.M44 * inverseDivider;

            return matrix1;
        }

        public static bool operator ==(Matrix4f matrix1, Matrix4f matrix2)
        {
            return (matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12) &&
                   (matrix1.M13 == matrix2.M13) && (matrix1.M14 == matrix2.M14) &&
                   (matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22) &&
                   (matrix1.M23 == matrix2.M23) && (matrix1.M24 == matrix2.M24) &&
                   (matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32) &&
                   (matrix1.M33 == matrix2.M33) && (matrix1.M34 == matrix2.M34) &&
                   (matrix1.M41 == matrix2.M41) && (matrix1.M42 == matrix2.M42) &&
                   (matrix1.M43 == matrix2.M43) && (matrix1.M44 == matrix2.M44);
        }

        public static bool operator !=(Matrix4f matrix1, Matrix4f matrix2)
        {
            return (matrix1.M11 != matrix2.M11) || (matrix1.M12 != matrix2.M12) ||
                   (matrix1.M13 != matrix2.M13) || (matrix1.M14 != matrix2.M14) ||
                   (matrix1.M21 != matrix2.M21) || (matrix1.M22 != matrix2.M22) ||
                   (matrix1.M23 != matrix2.M23) || (matrix1.M24 != matrix2.M24) ||
                   (matrix1.M31 != matrix2.M31) || (matrix1.M32 != matrix2.M32) ||
                   (matrix1.M33 != matrix2.M33) || (matrix1.M34 != matrix2.M34) ||
                   (matrix1.M41 != matrix2.M41) || (matrix1.M42 != matrix2.M42) ||
                   (matrix1.M43 != matrix2.M43) || (matrix1.M44 != matrix2.M44);
        }

        public static Matrix4f operator *(Matrix4f matrix1, Matrix4f matrix2)
        {
            Matrix4f result;

            result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
            result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
            result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
            result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

            result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
            result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
            result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
            result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

            result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
            result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
            result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
            result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

            result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
            result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
            result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
            result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;

            return result;
        }

        public static Matrix4f operator *(Matrix4f matrix, float scaleFactor)
        {
            matrix.M11 = matrix.M11 * scaleFactor;
            matrix.M12 = matrix.M12 * scaleFactor;
            matrix.M13 = matrix.M13 * scaleFactor;
            matrix.M14 = matrix.M14 * scaleFactor;
            matrix.M21 = matrix.M21 * scaleFactor;
            matrix.M22 = matrix.M22 * scaleFactor;
            matrix.M23 = matrix.M23 * scaleFactor;
            matrix.M24 = matrix.M24 * scaleFactor;
            matrix.M31 = matrix.M31 * scaleFactor;
            matrix.M32 = matrix.M32 * scaleFactor;
            matrix.M33 = matrix.M33 * scaleFactor;
            matrix.M34 = matrix.M34 * scaleFactor;
            matrix.M41 = matrix.M41 * scaleFactor;
            matrix.M42 = matrix.M42 * scaleFactor;
            matrix.M43 = matrix.M43 * scaleFactor;
            matrix.M44 = matrix.M44 * scaleFactor;
            return matrix;
        }

        public static Matrix4f operator *(float scaleFactor, Matrix4f matrix)
        {
            matrix.M11 = matrix.M11 * scaleFactor;
            matrix.M12 = matrix.M12 * scaleFactor;
            matrix.M13 = matrix.M13 * scaleFactor;
            matrix.M14 = matrix.M14 * scaleFactor;
            matrix.M21 = matrix.M21 * scaleFactor;
            matrix.M22 = matrix.M22 * scaleFactor;
            matrix.M23 = matrix.M23 * scaleFactor;
            matrix.M24 = matrix.M24 * scaleFactor;
            matrix.M31 = matrix.M31 * scaleFactor;
            matrix.M32 = matrix.M32 * scaleFactor;
            matrix.M33 = matrix.M33 * scaleFactor;
            matrix.M34 = matrix.M34 * scaleFactor;
            matrix.M41 = matrix.M41 * scaleFactor;
            matrix.M42 = matrix.M42 * scaleFactor;
            matrix.M43 = matrix.M43 * scaleFactor;
            matrix.M44 = matrix.M44 * scaleFactor;
            return matrix;
        }

        public static Matrix4f operator -(Matrix4f matrix1, Matrix4f matrix2)
        {
            matrix1.M11 -= matrix2.M11;
            matrix1.M12 -= matrix2.M12;
            matrix1.M13 -= matrix2.M13;
            matrix1.M14 -= matrix2.M14;
            matrix1.M21 -= matrix2.M21;
            matrix1.M22 -= matrix2.M22;
            matrix1.M23 -= matrix2.M23;
            matrix1.M24 -= matrix2.M24;
            matrix1.M31 -= matrix2.M31;
            matrix1.M32 -= matrix2.M32;
            matrix1.M33 -= matrix2.M33;
            matrix1.M34 -= matrix2.M34;
            matrix1.M41 -= matrix2.M41;
            matrix1.M42 -= matrix2.M42;
            matrix1.M43 -= matrix2.M43;
            matrix1.M44 -= matrix2.M44;
            return matrix1;
        }


        public static Matrix4f operator -(Matrix4f matrix)
        {
            matrix.M11 = -matrix.M11;
            matrix.M12 = -matrix.M12;
            matrix.M13 = -matrix.M13;
            matrix.M14 = -matrix.M14;
            matrix.M21 = -matrix.M21;
            matrix.M22 = -matrix.M22;
            matrix.M23 = -matrix.M23;
            matrix.M24 = -matrix.M24;
            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M33 = -matrix.M33;
            matrix.M34 = -matrix.M34;
            matrix.M41 = -matrix.M41;
            matrix.M42 = -matrix.M42;
            matrix.M43 = -matrix.M43;
            matrix.M44 = -matrix.M44;
            return matrix;
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            if (obj is Matrix4f)
                return this == (Matrix4f)obj;
            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "{ {M11:" + M11 + " M12:" + M12 + " M13:" + M13 + " M14:" + M14 + "}" +
                    " {M21:" + M21 + " M22:" + M22 + " M23:" + M23 + " M24:" + M24 + "}" +
                    " {M31:" + M31 + " M32:" + M32 + " M33:" + M33 + " M34:" + M34 + "}" +
                    " {M41:" + M41 + " M42:" + M42 + " M43:" + M43 + " M44:" + M44 + "} }";
        }

        #endregion

    }
}