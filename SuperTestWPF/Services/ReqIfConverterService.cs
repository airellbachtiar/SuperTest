using Microsoft.Extensions.Logging;
using ReqIFSharp;
using SuperTestWPF.Models;

namespace SuperTestWPF.Services
{
    public class ReqIFConverterService(ILogger<ReqIFConverterService> logger) : IReqIFConverterService
    {
        private readonly ILogger<ReqIFConverterService> _logger = logger;

        public ReqIF ConvertRequirementToReqIfAsync(IEnumerable<RequirementModel> requirements)
        {
            try
            {
                var reqIf = new ReqIF
                {
                    TheHeader = new ReqIFHeader()
                    {
                        Title = "Requirement",
                        CreationTime = DateTime.Now,
                        ReqIFToolId = "SuperTest",
                        SourceToolId = "Sioux",
                        RepositoryId = Guid.NewGuid().ToString(),
                    },
                    CoreContent = new ReqIFContent()
                };

                var textDataType = new DatatypeDefinitionString()
                {
                    Identifier = "dt-string-1",
                    LongName = "String Data Type"
                };

                var contentAttributeDefinition = new AttributeDefinitionString()
                {
                    LongName = "Content",
                    Identifier = "attr-content",
                    Type = textDataType
                };

                var traceAttributeDefinition = new AttributeDefinitionString()
                {
                    LongName = "Trace",
                    Identifier = "attr-trace",
                    Type = textDataType
                };

                var reqType = new SpecObjectType()
                {
                    Identifier = "requirement-type",
                    LongName = "Requirement"
                };
                reqType.SpecAttributes.Add(contentAttributeDefinition);
                reqType.SpecAttributes.Add(traceAttributeDefinition);

                foreach (var requirement in requirements)
                {
                    var req = new SpecObject()
                    {
                        Identifier = requirement.Id ?? Guid.NewGuid().ToString(),
                        LastChange = DateTime.Now,
                        Type = reqType
                    };
                    req.Values.Add(new AttributeValueString()
                    {
                        TheValue = requirement.Content,
                        Definition = contentAttributeDefinition
                    });
                    req.Values.Add(new AttributeValueString()
                    {
                        TheValue = requirement.Trace,
                        Definition = traceAttributeDefinition
                    });
                    reqIf.CoreContent.SpecObjects.Add(req);
                }

                reqIf.CoreContent.SpecTypes.Add(reqType);
                return reqIf;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while converting RequirementModel to ReqIF.");
                throw;
            }
        }
    }
}
