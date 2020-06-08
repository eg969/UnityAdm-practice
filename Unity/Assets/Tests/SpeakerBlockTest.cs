using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text;
using static BlockTypes;
using static AudioBlockWrapper;
namespace Tests
{
    public class SpeakerBlockTest
    {
        public byte newBlockFlag;
        public int objId;
        public int cfId;
        public int blockId;
        public float rTime;
        public float duration;
        public float x;
        public float y;
        public float z;
        public float azimuth;
        public float elevation;
        public float distance;
        public float azimuthMax;
        public float elevationMax;
        public float distanceMax;
        public float azimuthMin;
        public float elevationMin;
        public float distanceMin;
        public byte[] verticalEdge;
        public byte[] horizontalEdge;
        public int typeDef;
        public int channelNum;
        string filePath = "/Users/edgarsg/Desktop/panned_noise_adm.wav";
        byte[] byteArray;
		int newInvalidBlock;

		[SetUp]
        public void ReadFile()
        {
            byteArray = Encoding.ASCII.GetBytes(filePath + '\0');
            int validFile = readAdm(byteArray);
            if (validFile == 0)
            {
                AdmSpeakerAudioBlock nextBlock = getNextSpeakerBlock();

                newBlockFlag = nextBlock.newBlockFlag;
                objId = nextBlock.objId;
                cfId = nextBlock.cfId;
                blockId = nextBlock.blockId;
                typeDef = nextBlock.typeDef;
                rTime = nextBlock.rTime;
                duration = nextBlock.duration;
                x = nextBlock.x;
                y = nextBlock.y;
                z = nextBlock.z;
                azimuth = nextBlock.azimuth;
                elevation = nextBlock.elevation;
                distance = nextBlock.distance;
                azimuthMax = nextBlock.azimuthMax;
                elevationMax = nextBlock.elevationMax;
                distanceMax = nextBlock.distanceMax;
                azimuthMin = nextBlock.azimuthMin;
                elevationMin = nextBlock.elevationMin;
                distanceMin = nextBlock.distanceMin;
                verticalEdge = nextBlock.verticalEdge;
                horizontalEdge = nextBlock.horizontalEdge;
                typeDef = nextBlock.typeDef;
                channelNum = nextBlock.channelNum;

				nextBlock = getNextSpeakerBlock();
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
        public void NewBlockFlagTest()
        {
            Assert.AreEqual(1, newBlockFlag);
        }

        [Test]
        public void ObjIdTest()
        {
            Assert.AreEqual(1, objId);
        }

        [Test]
        public void CfIdTest()
        {
            Assert.AreEqual(1, cfId);
        }

        [Test]
        public void BlockIdTest()
        {
            Assert.AreEqual(1, blockId);
        }

        [Test]
        public void RTimeTest()
        {
            Assert.AreEqual(1.0, rTime);
        }

        [Test]
        public void DurationTest()
        {
            Assert.AreEqual(1.0, duration);
        }

        [Test]
        public void XTest()
        {
            Assert.AreEqual(1.0, x);
        }

        [Test]
        public void YTest()
        {
            Assert.AreEqual(1.0, y);
        }

        [Test]
        public void ZTest()
        {
            Assert.AreEqual(1.0, z);
        }

        [Test]
        public void AzimuthTest()
        {
            Assert.AreEqual(1.0, azimuth);
        }

        [Test]
        public void ElevationTest()
        {
            Assert.AreEqual(1.0, elevation);
        }

        [Test]
        public void DistanceTest()
        {
            Assert.AreEqual(1.0, distance);
        }

        [Test]
        public void AzimuthMaxTest()
        {
            Assert.AreEqual(1.0, azimuthMax);
        }

        [Test]
        public void ElevationMaxTest()
        {
            Assert.AreEqual(1.0, elevationMax);
        }

        [Test]
        public void DistanceMaxTest()
        {
            Assert.AreEqual(1.0, distanceMax);
        }

        [Test]
        public void AzimuthMinTest()
        {
            Assert.AreEqual(1.0, azimuthMin);
        }

        [Test]
        public void ElevationMinTest()
        {
            Assert.AreEqual(1.0, elevationMin);
        }

        [Test]
        public void distanceMinTest()
        {
            Assert.AreEqual(1.0, distanceMin);
        }

        [Test]
        public void TypeDefTest()
        {
            Assert.AreEqual(1, typeDef);
        }

        [Test]
        public void ChannelNumTest()
        {
            Assert.AreEqual(1, channelNum);
        }

		[Test]
		public void NewInvalidBlockTest()
		{
			Assert.AreEqual(0, newInvalidBlock);
		}
	}
}
