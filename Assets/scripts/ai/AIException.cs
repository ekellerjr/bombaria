using System;

public class AIException : SystemException {

    public AIException(string message) : base(message) { }

    public AIException(string message, Exception inner) : base(message, inner) { }

}
