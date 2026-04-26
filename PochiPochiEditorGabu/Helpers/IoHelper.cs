using PochiPochiEditorGabu.Constants;

namespace PochiPochiEditorGabu.Helpers
{
    public static class IoHelper
    {
        public static bool TryReadGbaPointer(uint ptrAddr, byte[] data, out uint? actualAddr)
        {
            uint rawPtr = (uint)data[ptrAddr] |
                         ((uint)data[ptrAddr + 1] << 8) |
                         ((uint)data[ptrAddr + 2] << 16) |
                         ((uint)data[ptrAddr + 3] << 24);

            if (rawPtr == 0)
            {
                actualAddr = null;
                return true;
            }

            if (rawPtr < GbaConstants.BaseAddr)
            {
                actualAddr = null;
                return false;
            }

            actualAddr = rawPtr - GbaConstants.BaseAddr;
            return true;
        }
    }
}
