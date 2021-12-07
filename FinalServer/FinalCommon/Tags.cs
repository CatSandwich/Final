namespace FinalCommon
{
    public enum ServerToClient : ushort
    {
        MoveObject,
        ResizeObject
    }

    public enum ClientToServer : ushort
    {
        PaddlePosition
    }
}
