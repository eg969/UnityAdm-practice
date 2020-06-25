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
    public class HoaBlockTest
    {
        public byte newBlockFlag;
        public int objId;
        public int cfId;
        public int blockId;
        public int typeDef;
        public float rTime;
        public float duration;
        public int channelNum;
        public int order;
        public int degree;
        public int numOfChannels;
        public float nfcRefDist;
        public int screenRef;
        public byte[] normalization;
        public byte[] equation;
        string filePath = "/Users/edgarsg/Desktop/hoa_block_test.wav";
        byte[] byteArray;
		int newInvalidBlock;

		[SetUp]
        public void ReadFile()
        {
            byteArray = Encoding.ASCII.GetBytes(filePath + '\0');
            int validFile = readAdm(byteArray);
            if (validFile == 0)
            {
                AdmHoaAudioBlock nextBlock = getNextHoaBlock();
                newBlockFlag = nextBlock.newBlockFlag;
                objId = nextBlock.objId;
                cfId = nextBlock.cfId;
                blockId = nextBlock.blockId;
                typeDef = nextBlock.typeDef;
                rTime = nextBlock.rTime;
                duration = nextBlock.duration;
                typeDef = nextBlock.typeDef;
                channelNum = nextBlock.channelNum;
                order = nextBlock.order;
                degree = nextBlock.degree;
                numOfChannels = nextBlock.numOfChannels;
                nfcRefDist = nextBlock.nfcRefDist;
                screenRef = nextBlock.screenRef;

				nextBlock = getNextHoaBlock();
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
            Assert.AreEqual(0, rTime);
        }

        [Test]
        public void DurationTest()
        {
            Assert.AreEqual(0, duration);
        }

        [Test]
        public void OrderTest()
        {
            Assert.AreEqual(0, order);
        }

        [Test]
        public void DegreeTest()
        {
            Assert.AreEqual(0, degree);
        }

        [Test]
        public void NumOfChannelsTest()
        {
            Assert.AreEqual(1, numOfChannels);
        }

        [Test]
        public void NfcRefDistTest()
        {
            Assert.AreEqual(0.0, nfcRefDist);
        }

        [Test]
        public void ScreenRefTest()
        {
            Assert.AreEqual(0, screenRef);
        }

        [Test]
        public void TypeDefTest()
        {
            Assert.AreEqual(1.0, typeDef);
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
