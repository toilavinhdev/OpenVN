using AutoMapper;
using OpenVN.Domain;
using SharedKernel.Libraries;
using SharedKernel.Providers;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class CreateSpendingCommandHandler : BaseCommandHandler, IRequestHandler<CreateSpendingCommand, string>
    {
        private readonly ISpendingWriteOnlyRepository _spendingWriteOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IApplicationConfiguration _appSettingConfiguration;
        private readonly AssetSettings _assetSettings;

        public CreateSpendingCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            ISpendingWriteOnlyRepository spendingWriteOnlyRepository,
            IMapper mapper,
            IApplicationConfiguration appSettingConfiguration
        ) : base(eventDispatcher, authService)
        {
            _spendingWriteOnlyRepository = spendingWriteOnlyRepository;
            _mapper = mapper;
            _appSettingConfiguration = appSettingConfiguration;
            _assetSettings = _appSettingConfiguration.GetConfiguration<AssetSettings>();
        }

        public async Task<string> Handle(CreateSpendingCommand request, CancellationToken cancellationToken)
        {
            //if (request.SpendingDto.Subs != null && request.SpendingDto.Subs.Any())
            //{
            //    foreach (var sub in request.SpendingDto.Subs)
            //    {
            //        var validate = await new SpendingDtoValidator().ValidateAsync(sub, cancellationToken);
            //        if (!validate.IsValid)
            //        {
            //            throw new BadRequestException(string.Join(", ", validate.Errors));
            //        }
            //    }
            //}

            //var tree = _mapper.Map<SpendingTree>(request.SpendingDto);
            //var deepLevel = 1;
            //var independents = Flatten(tree, deepLevel);
            //var rootNode = independents.First(s => string.IsNullOrEmpty(s.RefCode));
            //var entity = _mapper.Map<Spending>(rootNode);

            //independents.Remove(rootNode);
            //entity.AddDomainEvent(new InsertAuditEvent<Spending>((Spending)entity.Clone()));

            //var root = await _spendingWriteOnlyRepository.SaveAsync(entity, cancellationToken);
            //var parent = _mapper.Map<SpendingTree>(root);
            //parent.Code = rootNode.Code;

            //await AddSpendingsAsync(independents, parent, cancellationToken);
            //await _spendingWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            //return root.Id.ToString();
            return "";
        }

        private List<SpendingTree> Flatten(SpendingTree tree, int deepLevel)
        {
            var result = new List<SpendingTree> { tree };
            var code = Utility.RandomString(12);

            tree.Code = code;
            if (tree.Subs != null && tree.Subs.Any())
            {
                deepLevel++;
                if (deepLevel > _assetSettings.MaxDeepLevel)
                {
                    throw new BadRequestException($"We don't support deep level greater than {_assetSettings.MaxDeepLevel}");
                }

                foreach (var sub in tree.Subs)
                {
                    var independent = _mapper.Map<SpendingTree>(sub);
                    independent.RefCode = tree.Code;
                    result.AddRange(Flatten(independent, deepLevel));
                }
            }

            return result;
        }

        private async Task AddSpendingsAsync(List<SpendingTree> independents, SpendingTree parent, CancellationToken cancellationToken)
        {
            var children = independents.Where(s => s.RefCode == parent.Code).ToList();
            if (!children.Any())
                return;

            foreach (var child in children)
            {
                Spending result;
                independents.Remove(child);

                child.ParentId = parent.Id.ToString();
                child.Path = string.IsNullOrEmpty(parent.Path) ? parent.Id : $"{parent.Path}-{parent.Id}";

                result = await _spendingWriteOnlyRepository.SaveAsync(_mapper.Map<Spending>(child), cancellationToken);
                var next = _mapper.Map<SpendingTree>(result);
                next.Code = child.Code;

                await AddSpendingsAsync(independents, next, cancellationToken);
            }
        }
    }
}
