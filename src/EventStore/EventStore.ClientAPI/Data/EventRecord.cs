﻿// Copyright (c) 2012, Event Store LLP
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//  
// Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.
// Neither the name of the Event Store LLP nor the names of its
// contributors may be used to endorse or promote products derived from
// this software without specific prior written permission
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//  
using System;
using System.Linq;
using EventStore.ClientAPI.Common.Utils;
using EventStore.ClientAPI.Defines;

namespace EventStore.ClientAPI.Data
{
    //TODO GFY is this really what we want to expose to the world as an event through the API?
    //What does expected version mean here?
    public class EventRecord : IEquatable<EventRecord>
    {
        public static readonly byte[] Empty = new byte[0];

        public readonly int EventNumber;

        public readonly long LogPosition;
        public readonly Guid CorrelationId;
        public readonly Guid EventId;
        public readonly long TransactionPosition;
        public readonly string EventStreamId;
        public readonly int ExpectedVersion;
        public readonly DateTime TimeStamp;
        public readonly string EventType;
        public readonly byte[] Data;
        public readonly byte[] Metadata;

        private readonly Flags _flags; 


        public bool IsDeleteTombstone
        {
            get { return (_flags & Flags.DeleteTombstone) == Flags.DeleteTombstone;  }
        }

        public EventRecord(int eventNumber,
                           long logPosition,
                           Guid correlationId,
                           Guid eventId,
                           long transactionPosition,
                           string eventStreamId,
                           int expectedVersion,
                           DateTime timeStamp,
                           bool isDeleteTombstone,
                           string eventType,
                           byte[] data,
                           byte[] metadata) : this(eventNumber,
                                                   logPosition,
                                                   correlationId,
                                                   eventId,
                                                   transactionPosition,
                                                   eventStreamId,
                                                   expectedVersion,
                                                   timeStamp,
                                                   isDeleteTombstone ? Flags.DeleteTombstone : Flags.None,
                                                   eventType,
                                                   data,
                                                   metadata)
        {
        }

        internal EventRecord(int eventNumber,
                           long logPosition,
                           Guid correlationId,
                           Guid eventId,
                           long transactionPosition,
                           string eventStreamId,
                           int expectedVersion,
                           DateTime timeStamp,
                           Flags flags,
                           string eventType,
                           byte[] data,
                           byte[] metadata)
        {
            Ensure.Nonnegative(logPosition, "logPosition");
            Ensure.Nonnegative(transactionPosition, "transactionPosition");
            Ensure.NotNull(eventStreamId, "eventStreamId");
            Ensure.Nonnegative(eventNumber, "eventNumber");
            Ensure.NotEmptyGuid(eventId, "eventId");
            Ensure.NotNull(data, "data");

            EventNumber = eventNumber;
            LogPosition = logPosition;
            CorrelationId = correlationId;
            EventId = eventId;
            TransactionPosition = transactionPosition;
            EventStreamId = eventStreamId;
            ExpectedVersion = expectedVersion;
            TimeStamp = timeStamp;
            _flags = flags;
            EventType = eventType ?? string.Empty;
            Data = data;
            Metadata = metadata ?? Empty;
        }

        public bool Equals(EventRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventNumber == other.EventNumber
                   && LogPosition == other.LogPosition
                   && CorrelationId.Equals(other.CorrelationId)
                   && EventId.Equals(other.EventId)
                   && TransactionPosition == other.TransactionPosition
                   && string.Equals(EventStreamId, other.EventStreamId)
                   && ExpectedVersion == other.ExpectedVersion
                   && TimeStamp.Equals(other.TimeStamp)
                   && _flags.Equals(other._flags)
                   && string.Equals(EventType, other.EventType)
                   && Data.SequenceEqual(other.Data)
                   && Metadata.SequenceEqual(other.Metadata);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EventRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = EventNumber;
                hashCode = (hashCode * 397) ^ LogPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ CorrelationId.GetHashCode();
                hashCode = (hashCode * 397) ^ EventId.GetHashCode();
                hashCode = (hashCode * 397) ^ TransactionPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ EventStreamId.GetHashCode();
                hashCode = (hashCode * 397) ^ ExpectedVersion;
                hashCode = (hashCode * 397) ^ TimeStamp.GetHashCode();
                hashCode = (hashCode * 397) ^ _flags.GetHashCode();
                hashCode = (hashCode * 397) ^ EventType.GetHashCode();
                hashCode = (hashCode * 397) ^ Data.GetHashCode();
                hashCode = (hashCode * 397) ^ Metadata.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(EventRecord left, EventRecord right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EventRecord left, EventRecord right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("EventNumber: {0}, "
                                 + "LogPosition: {1}, "
                                 + "CorrelationId: {2}, "
                                 + "EventId: {3}, "
                                 + "TransactionPosition: {4}, "
                                 + "EventStreamId: {5}, "
                                 + "ExpectedVersion: {6}, "
                                 + "TimeStamp: {7}, "
                                 + "Flags: {8}, "
                                 + "EventType: {9}",
                                 EventNumber,
                                 LogPosition,
                                 CorrelationId,
                                 EventId,
                                 TransactionPosition,
                                 EventStreamId,
                                 ExpectedVersion,
                                 TimeStamp,
                                 _flags,
                                 EventType);
        }
    }
}