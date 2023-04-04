namespace Lb1_5;

internal class InvalidTypeException : Exception {
    public InvalidTypeException(String message): base(message) {}
}

internal class WagonNumberExistsException : Exception {
    public WagonNumberExistsException(String message): base(message) {}
}

internal class InvalidSoldTicketsCount : Exception {
    public InvalidSoldTicketsCount(String message): base(message) {}
}
