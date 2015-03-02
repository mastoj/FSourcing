module FSourcing

open System
open Chessie.ErrorHandling

type AggregateId = AggregateId of Guid
type Command<'TCommand> = AggregateId * 'TCommand

type Version = int
type Event<'TEvent> = AggregateId * 'TEvent

type GetEvents<'TEvent, 'TError> = AggregateId -> Result<AggregateId * Version * Event<'TEvent> list, 'TError>
type SaveEvents<'TEvent, 'TError> = AggregateId * Version * 'TEvent list -> Result<'TEvent list, 'TError>

type EvolveOne<'TAgg, 'TEvent, 'TError> = 'TEvent -> 'TAgg -> Result<'TAgg, 'TError>
type Handle<'TAgg, 'TCommand, 'TEvent, 'TError> = 'TAgg -> Command<'TCommand> -> Result<'TEvent list, 'TError>

type private Evolve<'TAgg, 'TEvent, 'TError> = 
    EvolveOne<'TAgg, 'TEvent, 'TError> -> 'TAgg -> AggregateId * Version * Event<'TEvent> list -> Result<(AggregateId * Version * 'TAgg), 'TError>
type private HandleWrapper<'TAgg, 'TCommand, 'TEvent, 'TError> = Handle<'TAgg, 'TCommand, 'TEvent, 'TError> -> Command<'TCommand> -> (AggregateId * Version * 'TAgg) -> Result<(AggregateId * Version * 'TEvent list), 'TError>

type Execute<'TCommand, 'TEvent, 'TError> = Command<'TCommand> -> Result<'TEvent list, 'TError>
type ExecuteBuilder<'TAgg, 'TCommand, 'TEvent, 'TError> = 
    GetEvents<'TEvent, 'TError> -> SaveEvents<'TEvent, 'TError> -> EvolveOne<'TAgg, 'TEvent, 'TError> -> 'TAgg -> Handle<'TAgg, 'TCommand, 'TEvent, 'TError> -> Execute<'TCommand, 'TEvent, 'TError>

let (evolve:Evolve<'TAgg, 'TEvent, 'TError>) = fun evolveOne defaultState (id, version, events) ->
    let unwrappedEvents = events |> List.map (fun (aggId, e) -> e)
    let agg = List.fold (fun s e -> s >>= evolveOne e) (ok defaultState) unwrappedEvents
    match agg with
    | Ok (a,msgs) -> Ok ((id, version, a), msgs)
    | Fail msgs -> Fail msgs

let (handleWrapper:HandleWrapper<'TAgg, 'TCommand, 'TEvent, 'TError>) = fun handle command aggRes ->
    let internalHandle (aggregateId, version, aggregate) = handle aggregate command
    let mapToTuple (aggregateId, version, aggregate) events = ok (aggregateId, version, events)
    aggRes |> internalHandle >>= mapToTuple aggRes

let (executeBuilder:ExecuteBuilder<'TAgg, 'TCommand, 'TEvent, 'TError>) = 
    fun getEvents saveEvents evolveOne initState handle ->
        let execute (aggregateId, command) =
            aggregateId
            |> getEvents
            >>= evolve evolveOne initState
            >>= handleWrapper handle (aggregateId, command)
            >>= saveEvents
        execute


/// Documentation for my library
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///
module Library = 
  
  /// Returns 42
  ///
  /// ## Parameters
  ///  - `num` - whatever
  let hello num = 42
