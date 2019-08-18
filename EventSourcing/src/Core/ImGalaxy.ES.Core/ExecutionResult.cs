using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class ExecutionResult : IExecutionResult
    {
        private static readonly ExecutionResult SuccessResult = new ExecutionResult(true, null);
        private static readonly ExecutionResult FailedResult = new ExecutionResult(false, null); 
        public static ExecutionResult Success() => SuccessResult;
        public static ExecutionResult Failed() => FailedResult;
        public static ExecutionResult Failed(IEnumerable<string> errors) => new ExecutionResult(false, errors);
        public static ExecutionResult Failed(params string[] errors) => new ExecutionResult(false, errors);
        public bool IsSuccess { get; }
        public readonly IEnumerable<string> Errors; 

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
