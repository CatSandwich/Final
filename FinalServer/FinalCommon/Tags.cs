namespace FinalCommon
{
    // Networked events the server sends to the client
    public enum ServerToClient : ushort
    {
        MoveObject,
        ResizeObject,
        SetOwnership
    }

    // Networked events the client sends to the server
    public enum ClientToServer : ushort
    {
        PaddlePosition
    }
}
