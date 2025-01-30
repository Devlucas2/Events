namespace AwesomeDevEvents.API.Entities;

public class DevEventSpeaker
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string TalkTtile { get; set; }

    public string TalkDescription { get; set; } 

    public string LinkedinProfile { get; set; } 

    public Guid DevEventsId { get; set; }

}