using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleVoiceEmailHandlerUnitTests.Helpers
{
    public static class AssertExtensions
    {
        /// <summary>
        /// Verify that the given lambda throws an exception of the given type.
        /// </summary>
        /// <param name="expectedExceptionType"></param>
        /// <param name="actualAction"></param>
        public static void Throws(Type expectedExceptionType, Action actualAction)
        {
            ThrowsInternal(expectedExceptionType, null, actualAction);
        }

        /// <summary>
        /// Verify that the given lambda throws an exception of the given type with the given message.
        /// </summary>
        /// <param name="expectedExceptionType"></param>
        /// <param name="expectedMessage"></param>
        /// <param name="actualAction"></param>
        public static void Throws(Type expectedExceptionType, string expectedMessage, Action actualAction)
        {
            ThrowsInternal(expectedExceptionType, expectedMessage, actualAction);
        }

        private static void ThrowsInternal(Type expectedExceptionType, string expectedMessage, Action actualAction)
        {
            bool thrown = false;

            try
            {
                actualAction();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(expectedExceptionType, ex.GetType());

                if (expectedMessage != null)
                    Assert.AreEqual(expectedMessage, ex.Message);

                thrown = true;
            }

            if (!thrown)
                Assert.Fail("No exception thrown");
        }
    }
}
