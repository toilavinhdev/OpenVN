using Newtonsoft.Json;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Application
{
    public class AnalyseTextCommandHandler : BaseCommandHandler, IRequestHandler<AnalyseTextCommand, ChatContentDto>
    {
        private readonly IChatGeneratorWriteOnlyRepository _chatGeneratorWriteOnlyRepository;

        public AnalyseTextCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IChatGeneratorWriteOnlyRepository chatGeneratorWriteOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _chatGeneratorWriteOnlyRepository = chatGeneratorWriteOnlyRepository;
        }

        public async Task<ChatContentDto> Handle(AnalyseTextCommand request, CancellationToken cancellationToken)
        {
            var file = request.File;
            var extension = Path.GetExtension(file.FileName);

            if (extension != ".txt")
            {
                throw new BadRequestException("Only txt file is allowed");
            }
            var reader = new StreamReader(file.OpenReadStream());
            var contents = new List<ChatUserDto>();
            var line = string.Empty;
            var index = 0;
            var lastAt = 0;
            var markAt = 0;

            var content = new ChatUserDto();
            while ((line = reader.ReadLine()) != null)
            {
                var origin = line;
                try
                {
                    line = line.Trim().TrimEnd('.').Replace("\\n", "");
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (index++ % 2 == 0)
                    {
                        var header = line.Split(" ");
                        var author = header[0];
                        var times = header[1].Split(":");
                        var at = int.Parse(times[0]) * 3600 + int.Parse(times[1]) * 60 + int.Parse(times[2]);

                        // Trường hợp at < lastAt chứng tỏ đoạn này ghép file
                        if (at >= lastAt)
                        {
                            lastAt = at;
                        }
                        else
                        {
                            if (at < markAt)
                            {
                                markAt = 0;
                            }
                            lastAt += at - markAt;
                            markAt = at;
                        }

                        content.Key = author;
                        content.At = lastAt;
                    }
                    else
                    {
                        content.Content = line;
                        contents.Add(content);
                        content = new ChatUserDto();
                    }
                }
                catch (Exception ex)
                {
                    throw new BadRequestException($"Sai format tại: '{origin}' - message = {ex.Message}");
                }

            }
            reader.Close();
            var result = new ChatContentDto { Contents = contents };
            var entity = new ChatGenerator();
            entity.FileName = file.FileName;
            entity.Contents = JsonConvert.SerializeObject(result);

            await _chatGeneratorWriteOnlyRepository.SaveAsync(entity, cancellationToken);
            await _chatGeneratorWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return result;
        }
    }
}
