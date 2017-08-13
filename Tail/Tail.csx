#r "System.Reactive"
#r "Microsoft.Extensions.Logging.Abstractions"

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using Foopipes.Abstractions.Services;
using Foopipes.Abstractions.Exceptions;
using Foopipes.Core.Extensions;
using Microsoft.Extensions.Logging;

class TailService : ServiceBase, IObservableService, IRunnableService
{
    public string Filename => Config["filename"];

    public IObservable<ServiceEvent> Observable { get; private set; }
    private Subject<ServiceEvent> _subject;
    private ILogger _logger;

    public TailService(ILogger<TailService> logger=null)
    {
        _logger = logger;
    }

    public override Task Start()
    {
        _subject = new Subject<ServiceEvent>();
        Observable = _subject;
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Opening file {0}", Filename);
        try
        {
            using (var filestream = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(filestream))
            {
                long lastMaxOffset = reader.BaseStream.Length;

                //seek to the end
                reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                while (!cancellationToken.IsCancellationRequested)
                {
                    //if the file size has not changed, idle
                    if (reader.BaseStream.Length == lastMaxOffset)
                    {
                        // First attempt, 0ms delay (context switch)
                        if (reader.BaseStream.Length == lastMaxOffset)
                        {
                            await System.Threading.Tasks.Task.Delay(0, cancellationToken);
                        }
                        // Second attempt, 100ms delay
                        if (reader.BaseStream.Length == lastMaxOffset)
                        {
                            await System.Threading.Tasks.Task.Delay(100, cancellationToken);
                        }
                        continue;
                    }

                    //if the file is shorter than before, move pointer to current position
                    if (reader.BaseStream.Length < lastMaxOffset)
                    {
                        lastMaxOffset = reader.BaseStream.Length;

                        //seek to the end
                        reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);
                        continue;
                    }

                    //read out of the file until the EOF
                    string line;
                    while(!cancellationToken.IsCancellationRequested && (line = await reader.ReadLineAsync())!=null)
                    { 
                        _logger?.LogDebug("Got line {0} chars", line.Length);

                        var metadata = JObject.FromObject(new
                        {
                            filename = Filename,
                            position = filestream.Position,
                            length = filestream.Length,
                            encoding = reader.CurrentEncoding.ToString(),
                            endOfStream = reader.EndOfStream
                        });

                        _subject.OnNext(new ServiceEvent(this, metadata, new[] { new BinaryData(Encoding.UTF8.GetBytes(line)) }));
                    }

                    //update the last max offset
                    lastMaxOffset = reader.BaseStream.Position;
                }
            }
        }
        catch (FileNotFoundException e)
        {
            _logger?.LogError("File not found {0}", e.FileName);
        }
    }
}

Service.Register("tail", typeof(TailService));

