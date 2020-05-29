using System.Collections.Generic; 

namespace System
{
    public class ExecutionResult : IExecutionResult
    { 
        public static ExecutionResult Success = new ExecutionResult(true, null);
        public static ExecutionResult Failed = new ExecutionResult(false, null);
        public static ExecutionResult Fail(IEnumerable<string> errors) => new ExecutionResult(false, errors);
        public static ExecutionResult Fail(params string[] errors) => new ExecutionResult(false, errors);
        public bool IsSuccess { get; }
        public IEnumerable<string> Errors { get; }  
        private ExecutionResult(bool isSuccess, IEnumerable<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }
        public override string ToString()
        {
            return $"ExecutionResult - IsSuccess:{IsSuccess}";
        }
    }
}
