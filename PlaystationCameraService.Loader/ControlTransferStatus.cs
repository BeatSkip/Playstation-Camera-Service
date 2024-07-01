using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaystationCameraService.Loader;

internal enum ControlTransferStatus : int
{
    NO_ERROR = 0,
    ERROR_TIMEOUT = 1460,
    ERROR_OPERATION_ABORTED = 995,
    ERROR_GEN_FAILURE = 31,
    ERROR_HANDLE_EOF = 38,
    UNKNOWN_ERROR = -1,
}
