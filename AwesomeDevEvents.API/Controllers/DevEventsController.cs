﻿using AutoMapper;
using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Models;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers;
[Route("api/dev-events")]
[ApiController]
public class DevEventsController : ControllerBase
{
    private readonly DevEventsDbContext _context;
    private readonly IMapper _mapper;

    public DevEventsController(
        DevEventsDbContext context,
        IMapper mapper)
    {   
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Obter todos os eventos
    /// </summary>
    /// <returns>Coleção de eventos</returns>
    /// <response code="200">Sucesso</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();

        var viewModel= _mapper.Map<List<DevEventViewModel>>(devEvents);
        return Ok(viewModel);
    }

    /// <summary>
    /// Obter um evento
    /// </summary>
    /// <param name="id">Identificador do evento</param>
    /// <returns>Dados do evento</returns>
    /// <response code="200">Sucesso</response>
    /// <response code="404">Não encontrado</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var devEvent = _context.DevEvents
            .Include(de => de.Speakers)
            .SingleOrDefault(d => d.Id == id);

        if (devEvent == null)
        {
            return NotFound();
        }

        var viewModel = _mapper.Map<DevEventViewModel>(devEvent);

        return Ok(viewModel);
    }

    /// <summary>
    /// Cadastrar um evento 
    /// </summary>
    /// <remarks>
    /// {"title": "string","description": "string","startDate": "2025-01-22T23:06:56.500Z","endDate": "2025-01-22T23:06:56.501Z","speakers": [{"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6","name": "string","talkTtile": "string","talkDescription": "string","linkedinProfile": "string","devEventsId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"}],"isDeleted": true}
    /// </remarks>
    /// <param name="input">Dados do evento</param>
    /// <returns>Objeto recém-criado</returns>
    /// <response code="201">Sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Post(DevEventInputModel input)
    {   
        var devEvent = _mapper.Map<DevEvent>(input);
        _context.DevEvents.Add(devEvent);
        _context.SaveChanges();
       
        return CreatedAtAction(nameof(GetById), new {id = devEvent.Id}, devEvent);
    }

    /// <summary>
    /// Atualizar um evento
    /// </summary>
    /// <remarks>
    /// {"title": "string","description": "string","startDate": "2025-01-22T23:06:56.500Z","endDate": "2025-01-22T23:06:56.501Z","speakers": [{"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6","name": "string","talkTtile": "string","talkDescription": "string","linkedinProfile": "string","devEventsId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"}],"isDeleted": true}
    /// </remarks>
    /// <param name="id">Identificador do evento</param>
    /// <param name="input">Dados do evento</param>
    /// <returns>Nada.</returns>
    /// <response code="404">Não encontrado.</response>
    /// <response code="204">Sucesso</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]

    public IActionResult Update(Guid id ,DevEventInputModel input)
    {
        var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

        if (devEvent == null)
        {
            return NotFound();
        }

        devEvent.UpDate(input.Title, input.Description, input.StartDate, input.EndDate);

        _context.DevEvents.Update(devEvent);
        _context.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// Deletar um evento 
    /// </summary>
    /// <param name="id">Identificador de evento</param>
    /// <returns>Nada.</returns>
    /// <response code="404">Não encontrado</response>
    /// <responde code="204">Sucesso</responde>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(Guid id) 
    {
        var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

        if (devEvent == null)
        {
            return NotFound();
        }

        devEvent.Delete();

        _context.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// Cadastrar palestrante
    /// </summary>
    /// <remarks>
    /// {"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6","name": "string","talkTtile": "string","talkDescription": "string","linkedinProfile": "string","devEventsId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"}
    /// </remarks>
    /// <param name="id">Identificador do evento</param>
    /// <param name="input">Dados do palestrante</param>
    /// <returns>Nada</returns>
    /// <response code="204">Sucesso</response>
    /// <response code="404">Evento não encontrado</response>
    [HttpPost("{id}/speakers")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult PostSpeakers(Guid id,DevEventSpeakerInputModel input)
    {
        var speaker = _mapper.Map<DevEventSpeaker> (input);
        speaker.DevEventsId = id;

        var devEvent = _context.DevEvents.Any(d => d.Id == id);

        if (!devEvent)
        {
            return NotFound();
        }

        _context.DevEventsSpeaker.Add(speaker);
        _context.SaveChanges();

        return NoContent();
    }

}
