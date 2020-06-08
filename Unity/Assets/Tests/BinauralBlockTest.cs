using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static BlockTypes;
using static AudioBlockWrapper;

namespace Tests
{
    public class BinauralBlockTestS
    {
        public byte newBlockFlag;
        public int objId;
        public int cfId;
        public int blockId;
        public int typeDef;
        public float rTime;
        public float duration;
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
                AdmBinauralAudioBlock nextBlock = getNextBinauralBlock();

                newBlockFlag = nextBlock.newBlockFlag;
                objId = nextBlock.objId;
                cfId = nextBlock.cfId;
                blockId = nextBlock.blockId;
                typeDef = nextBlock.typeDef;
                rTime = nextBlock.rTime;
                duration = nextBlock.duration;
                typeDef = nextBlock.typeDef;
                channelNum = nextBlock.channelNum;

				nextBlock = getNextBinauralBlock();
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
