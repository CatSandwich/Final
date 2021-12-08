namespace FinalCommon
{
    public enum ServerToClient : ushort
    {
        MoveObject,
        ResizeObject,
        SetOwnership
    }

    public enum ClientToServer : ushort
    {
        PaddlePosition
    }
}
