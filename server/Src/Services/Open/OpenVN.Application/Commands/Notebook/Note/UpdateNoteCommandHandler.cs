using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Libraries;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class UpdateNoteCommandHandler : BaseCommandHandler, IRequestHandler<UpdateNoteCommand, Unit>
    {
        private readonly INoteWriteOnlyRepository _noteWriteOnlyRepository;
        private readonly INoteReadOnlyRepository _noteReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Resources> _localizer;

        public UpdateNoteCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            INoteWriteOnlyRepository noteWriteOnlyRepository,
            INoteReadOnlyRepository noteReadOnlyRepository,
            IMapper mapper,
            IStringLocalizer<Resources> localizer
        ) : base(eventDispatcher, authService)
        {
            _noteWriteOnlyRepository = noteWriteOnlyRepository;
            _noteReadOnlyRepository = noteReadOnlyRepository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
        {
            ValidateAndThrow(request.ChangeNoteDto, request.Type);

            var entity = _mapper.Map<Note>(request.ChangeNoteDto);

            var fieldChanges = new List<string>();
            foreach (var @enum in typeof(ChangeNoteType).GetEnumerationValues())
            {
                if ((request.Type & (int)@enum) == (int)@enum)
                {
                    fieldChanges.Add(@enum.ToString());
                }
            }

            // Handle riêng case change order
            if ((request.Type & (int)ChangeNoteType.Order)== (int)ChangeNoteType.Order)
            {
                var oldEntity = await _noteReadOnlyRepository.GetByIdAsync<Note>(entity.Id.ToString(), cancellationToken)
                                ?? throw new BadRequestException(_localizer["common_data_does_not_exist_or_was_deleted"].Value);


                // Update lại các order ở category cũ và mới
                if (oldEntity.CategoryId != entity.CategoryId)
                {
                    await _noteWriteOnlyRepository.UpdateFromIndexOrderToLastAsync(oldEntity.Order, 0, oldEntity.CategoryId, false, cancellationToken);
                    await _noteWriteOnlyRepository.UpdateFromIndexOrderToLastAsync(entity.Order, 0, entity.CategoryId, true, cancellationToken);
                }
                else
                {
                    var isIncrease = entity.Order < oldEntity.Order;
                    var fromOrder = Math.Min(entity.Order, oldEntity.Order);
                    var toOrder = Math.Max(entity.Order, oldEntity.Order);
                    if (!isIncrease)
                    {
                        fromOrder++;
                    }
                    else
                    {
                        toOrder--;
                    }

                    await _noteWriteOnlyRepository.UpdateFromIndexOrderToLastAsync(fromOrder, toOrder, entity.CategoryId, isIncrease, cancellationToken);
                }
            }

            await _noteWriteOnlyRepository.UpdateAsync(entity, fieldChanges, cancellationToken: cancellationToken);
            await _noteWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return Unit.Value;
        }

        private void ValidateAndThrow(ChangeNoteDto changeNoteDto, int type)
        {
            if (changeNoteDto == null)
            {
                throw new BadRequestException("Payload must not be null");
            }

            if ((type & (int)ChangeNoteType.Title) == (int)ChangeNoteType.Title)
            {
                if (string.IsNullOrEmpty(changeNoteDto.Title))
                {
                    throw new BadRequestException(_localizer["note_title_must_not_be_empty"].Value);
                }
            }
            if ((type & (int)ChangeNoteType.Content) == (int)ChangeNoteType.Content)
            {
                if (string.IsNullOrEmpty(changeNoteDto.Content))
                {
                    throw new BadRequestException(_localizer["note_content_must_not_be_empty"].Value);
                }
            }
            if ((type & (int)ChangeNoteType.CategoryId) == (int)ChangeNoteType.CategoryId)
            {
                if (string.IsNullOrEmpty(changeNoteDto.CategoryId) || !long.TryParse(changeNoteDto.CategoryId, out var id) || id <= 0)
                {
                    throw new BadRequestException(_localizer["note_category_id_must_not_be_empty_or_invalid"].Value);
                }
            }
            if ((type & (int)ChangeNoteType.Order) == (int)ChangeNoteType.Order)
            {
                if (changeNoteDto.Order <= 0)
                {
                    throw new BadRequestException(_localizer["note_order_must_greater_than_0"].Value);
                }
            }
            if ((type & (int)ChangeNoteType.BackgroundColor) == (int)ChangeNoteType.BackgroundColor)
            {
                if (string.IsNullOrEmpty(changeNoteDto.BackgroundColor))
                {
                    throw new BadRequestException(_localizer["note_background_must_not_be_empty"].Value);
                }
            }
        }
    }
}
