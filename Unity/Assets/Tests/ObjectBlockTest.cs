using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text;
using static BlockTypes;
using static AudioBlockWrapper;
using System;
namespace Tests
{
    public class ObjectBlockTest
    {
		private int newBlockFlag1;
		private int objId1;
		private int cfId1;
		private int blockId1;
		private int typeDef1;
		private float rTime1;
		private float duration1;
		private float interpolationLength1;
		private float x1;
		private float y1;
		private float z1;
		private int importance1;
		private float width1;
		private float height1;
		private float depth1;
		private float diffuse1;
		private float divergence1;
		private float maxDistance1;
		private float positionRange1;
		private float azimuthRange1;
		private int channelLock1;
		private float gain1;
		private int jumpPosition1;
		private int moveSpherically1;
		private int channelNum1;

        private int newBlockFlag2;
        private int objId2;
        private int cfId2;
        private int blockId2;
        private int typeDef2;
        private float rTime2;
        private float duration2;
        private float interpolationLength2;
        private float x2;
        private float y2;
        private float z2;
        private int importance2;
        private float width2;
        private float height2;
        private float depth2;
        private float diffuse2;
        private float divergence2;
        private float maxDistance2;
        private float positionRange2;
        private float azimuthRange2;
        private int channelLock2;
        private float gain2;
        private int jumpPosition2;
        private int moveSpherically2;
        private int channelNum2;

        string filePath = "/Users/edgarsg/Desktop/object_block_test.wav";
        byte[] byteArray;
		int newInvalidBlock;

        [SetUp]
		public void ReadFile()
        {
			byteArray = Encoding.ASCII.GetBytes(filePath + '\0');
			int validFile = readAdm(byteArray);
			if (validFile == 0)
			{
				AdmObjectsAudioBlock nextBlock = getNextObjectBlock();

				newBlockFlag1 = nextBlock.newBlockFlag;
				objId1 = nextBlock.objId;
				cfId1 = nextBlock.cfId;
				blockId1 = nextBlock.blockId;
				typeDef1 = nextBlock.typeDef;
				rTime1 = nextBlock.rTime;
				duration1 = nextBlock.duration;
				interpolationLength1 = nextBlock.interpolationLength;
				x1 = nextBlock.x;
				y1 = nextBlock.y;
				z1 = nextBlock.z;
				importance1 = nextBlock.importance;
				width1 = nextBlock.width;
				height1 = nextBlock.height;
				depth1 = nextBlock.depth;
				diffuse1 = nextBlock.diffuse;
				divergence1 = nextBlock.divergence;
				maxDistance1 = nextBlock.maxDistance;
				positionRange1 = nextBlock.positionRange;
				azimuthRange1 = nextBlock.azimuthRange;
				channelLock1 = nextBlock.channelLock;
				gain1 = nextBlock.gain;
				jumpPosition1 = nextBlock.jumpPosition;
				moveSpherically1 = nextBlock.moveSpherically;
				channelNum1 = nextBlock.channelNum;

                nextBlock = getNextObjectBlock();

                newBlockFlag2 = nextBlock.newBlockFlag;
                objId2 = nextBlock.objId;
                cfId2 = nextBlock.cfId;
                blockId2 = nextBlock.blockId;
                typeDef2 = nextBlock.typeDef;
                rTime2 = nextBlock.rTime;
                duration2 = nextBlock.duration;
                interpolationLength2 = nextBlock.interpolationLength;
                x2 = nextBlock.x;
                y2 = nextBlock.y;
                z2 = nextBlock.z;
                importance2 = nextBlock.importance;
                width2 = nextBlock.width;
                height2 = nextBlock.height;
                depth2 = nextBlock.depth;
                diffuse2 = nextBlock.diffuse;
                divergence2 = nextBlock.divergence;
                maxDistance2 = nextBlock.maxDistance;
                positionRange2 = nextBlock.positionRange;
                azimuthRange2 = nextBlock.azimuthRange;
                channelLock2 = nextBlock.channelLock;
                gain2 = nextBlock.gain;
                jumpPosition2 = nextBlock.jumpPosition;
                moveSpherically2 = nextBlock.moveSpherically;
                channelNum2 = nextBlock.channelNum;

                nextBlock = getNextObjectBlock();

				newInvalidBlock = nextBlock.newBlockFlag;

			}
		}

        [Test]
        public void ReadFileTest()
        {
            byteArray = Encoding.ASCII.GetBytes(filePath + '\0');
            int validFile = readAdm(byteArray);
            Assert.AreEqual(0, validFile);
        }

        [Test]
        public void NewBlockFlagTest1()
        {
            Assert.AreEqual(1, newBlockFlag1);
        }

		[Test]
		public void ObjIdTest1()
		{
			Assert.AreEqual(1, objId1);
		}

		[Test]
		public void CfIdTest1()
		{
			Assert.AreEqual(1, cfId1);
		}

		[Test]
		public void BlockIdTest1()
		{
			Assert.AreEqual(1, blockId1);
		}

		[Test]
		public void RTimeTest1()
		{
			Assert.AreEqual(0.23, rTime1);
		}

		[Test]
		public void DurationTest1()
		{
			Assert.AreEqual(3.6, duration1);
		}

		[Test]
		public void InterpolationLengthTest1()
		{
			Assert.AreEqual(13.79, interpolationLength1);
		}

		[Test]
		public void XTest1()
		{
			Assert.AreEqual(1.23, x1);
		}

		[Test]
		public void YTest1()
		{
			Assert.AreEqual(3.12, y1);
		}

		[Test]
		public void ZTest1()
		{
			Assert.AreEqual(4.21, z1);
		}

		[Test]
		public void ImportanceTest1()
		{
			Assert.AreEqual(4, importance1);
		}

		[Test]
        public void WidthTest1()
		{
			Assert.AreEqual(13.0, width1);
		}

		[Test]
		public void HeightTest1()
		{
			Assert.AreEqual(25.0, height1);
		}

		[Test]
		public void DepthTest1()
		{
			Assert.AreEqual(57.0, depth1);
		}

		[Test]
		public void DiffuseTest1()
		{
			Assert.AreEqual(0.56, diffuse1);
		}
		[Test]
		public void DivergenceTest1()
		{
			Assert.AreEqual(0.25, divergence1);
		}

		[Test]
		public void MaxDistanceTest1()
		{
			Assert.AreEqual(1.9, maxDistance1);
		}

		[Test]
		public void PositionRangeTest1()
		{
			Assert.AreEqual(0.515, positionRange1);
		}

		[Test]
		public void AzimuthRangeTest1()
		{
			Assert.AreEqual(151.0, azimuthRange1);
		}

		[Test]
        public void ChannelLockTest1()
		{
			Assert.AreEqual(1, channelLock1);
		}

		[Test]
		public void GainTest1()
		{
			Assert.AreEqual(56.0, gain1);
		}

		[Test]
		public void JumpPositionTest1()
		{
			Assert.AreEqual(1, jumpPosition1);
		}

		[Test]
		public void MoveSphericallyTest1()
		{
			Assert.AreEqual(0, moveSpherically1);
		}

        [Test]
        public void TypeDefTest1()
        {
            Assert.AreEqual(1, typeDef1);
        }

        [Test]
		public void ChannelNumTest1()
		{
			Assert.AreEqual(1, channelNum1);
		}

        [Test]
        public void NewBlockFlagTest2()
        {
            Assert.AreEqual(1, newBlockFlag2);
        }

        [Test]
        public void ObjIdTest2()
        {
            Assert.AreEqual(1, objId2);
        }

        [Test]
        public void CfIdTest2()
        {
            Assert.AreEqual(1, cfId2);
        }

        [Test]
        public void BlockIdTest2()
        {
            Assert.AreEqual(1, blockId2);
        }

        [Test]
        public void RTimeTest2()
        {
            Assert.AreEqual(60.0, rTime2);
        }

        [Test]
        public void DurationTest2()
        {
            Assert.AreEqual(0.1, duration2);
        }

        [Test]
        public void InterpolationLengthTest2()
        {
            Assert.AreEqual(0.0, interpolationLength2);
        }

        [Test]
        public void XTest2()
        {
            Assert.AreEqual(1.0, x2);
        }

        [Test]
        public void YTest2()
        {
            Assert.AreEqual(1.0, y2);
        }

        [Test]
        public void ZTest2()
        {
            Assert.AreEqual(1.0, z2);
        }

        [Test]
        public void ImportanceTest2()
        {
            Assert.AreEqual(7.0, importance2);
        }

        [Test]
        public void WidthTest2()
        {
            Assert.AreEqual(26.0, width2);
        }

        [Test]
        public void HeightTest2()
        {
            Assert.AreEqual(50.0, height2);
        }

        [Test]
        public void DepthTest2()
        {
            Assert.AreEqual(104.0, depth2);
        }

        [Test]
        public void DiffuseTest2()
        {
            Assert.AreEqual(0.44, diffuse2);
        }
        [Test]
        public void DivergenceTest2()
        {
            Assert.AreEqual(0.265, divergence2);
        }

        [Test]
        public void MaxDistanceTest2()
        {
            Assert.AreEqual(0.21, maxDistance2);
        }

        [Test]
        public void PositionRangeTest2()
        {
            Assert.AreEqual(0.52, positionRange2);
        }

        [Test]
        public void AzimuthRangeTest2()
        {
            Assert.AreEqual(32.0, azimuthRange2);
        }

        [Test]
        public void ChannelLockTest2()
        {
            Assert.AreEqual(0, channelLock2);
        }

        [Test]
        public void GainTest2()
        {
            Assert.AreEqual(13.5, gain2);
        }

        [Test]
        public void JumpPositionTest2()
        {
            Assert.AreEqual(0, jumpPosition2);
        }

        [Test]
        public void MoveSphericallyTest2()
        {
            Assert.AreEqual(1, moveSpherically2);
        }

        [Test]
        public void TypeDefTest2()
        {
            Assert.AreEqual(1, typeDef2);
        }

        [Test]
        public void ChannelNumTest2()
        {
            Assert.AreEqual(1, channelNum2);
        }

        [Test]
		public void NewInvalidBlockTest()
		{
			Assert.AreEqual(0, newInvalidBlock);
		}
	}
}
