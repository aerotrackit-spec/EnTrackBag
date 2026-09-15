namespace EnTrackBag.Api.Data.Entities;
public class ControllerEntity { public int ID {get;set;} public string? ControllerName {get;set;} public string? ControllerIP {get;set;} public DateTime? LastConnected {get;set;} public DateTime? LastDisconnected {get;set;} public string? LastError {get;set;} public string? Status {get;set;} public bool? IsActive {get;set;} }
