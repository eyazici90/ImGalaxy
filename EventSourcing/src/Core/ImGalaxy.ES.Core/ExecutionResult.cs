﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImGalaxy.ES.Core
{
    public class ExecutionResult : IExecutionResult
    { 
        public static ExecutionResult Success = new ExecutionResult(true, null);
        public static ExecutionResult Failed = new ExecutionResult(false, null);
        public static ExecutionResult Fail(IEnumerable<string> errors) => new ExecutionResult(false, errors);
        public static ExecutionResult Fail(params string[] errors) => new ExecutionResult(false, errors);
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
