using Grpc.Core;
using NetifeMessage;

namespace Netife_cli.Services;

public class FrontendService : NetifeMessage.NetifePost.NetifePostBase
{
    public static Func<NetifeMessage.NetifeProbeRequest, NetifeMessage.NetifeProbeResponse> action 
        = new Func<NetifeProbeRequest, NetifeProbeResponse>(sp =>
        {
            var res = new NetifeProbeResponse();
            res.Uuid = sp.Uuid;
            res.DstIpAddr = sp.DstIpAddr;
            res.DstIpPort = sp.DstIpPort;
            res.ResponseText = sp.RawText;
            return res;
        });
    public override async Task<NetifeMessage.NetifeProbeResponse>
        UploadRequest(NetifeMessage.NetifeProbeRequest request, ServerCallContext context)
    {
        return action(request);
    }
}