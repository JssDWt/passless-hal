using System;
using Newtonsoft.Json;

namespace Passless.Hal.Streaming
{
    public class WrappedJsonReader : JsonReader
    {
        protected bool IsDisposed { get; private set; }
        protected JsonReader innerReader;

        public WrappedJsonReader(JsonReader inner)
        {
            this.innerReader = inner
                ?? throw new ArgumentNullException(nameof(inner));

            // NOTE: These properties are not virtual, so have to be set manually.
            this.Culture = inner.Culture;
            // this.CloseInput = inner.CloseInput; // CloseInput defines whether the inner reader is closed.
            this.SupportMultipleContent = inner.SupportMultipleContent;
            this.DateTimeZoneHandling = inner.DateTimeZoneHandling;
            this.FloatParseHandling = inner.FloatParseHandling;
            this.DateFormatString = inner.DateFormatString;
            this.MaxDepth = inner.MaxDepth;
            this.DateParseHandling = inner.DateParseHandling;
        }

        public override Type ValueType => this.innerReader.ValueType;
        public override object Value => this.innerReader.Value;
        public override char QuoteChar => this.innerReader.QuoteChar;
        public override string Path => this.innerReader.Path;
        public override int Depth => this.innerReader.Depth;
        public override JsonToken TokenType => this.innerReader.TokenType;

        public override void Close()
        {
            if (!this.IsDisposed)
            {
                base.Close();

                if (this.CloseInput)
                {
                    this.innerReader.Close();
                    this.innerReader = null;
                }

                this.IsDisposed = true;
            }

        }
        public override bool Read()
        {
            this.ThrowIfDisposed();
            return this.innerReader.Read();
        }

        // public override bool? ReadAsBoolean() 
        //     => this.innerReader.ReadAsBoolean();
        // public override Task<bool?> ReadAsBooleanAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsBooleanAsync(cancellationToken);
        // public override byte[] ReadAsBytes() 
        //     => this.innerReader.ReadAsBytes();
        // public override Task<byte[]> ReadAsBytesAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsBytesAsync(cancellationToken);
        // public override DateTime? ReadAsDateTime()
        //     => this.innerReader.ReadAsDateTime();
        // public override Task<DateTime?> ReadAsDateTimeAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsDateTimeAsync(cancellationToken);
        // public override DateTimeOffset? ReadAsDateTimeOffset()
        //     => this.innerReader.ReadAsDateTimeOffset();
        // public override Task<DateTimeOffset?> ReadAsDateTimeOffsetAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsDateTimeOffsetAsync(cancellationToken);
        // public override decimal? ReadAsDecimal() 
        //     => this.innerReader.ReadAsDecimal();
        // public override Task<decimal?> ReadAsDecimalAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsDecimalAsync(cancellationToken);
        // public override double? ReadAsDouble()
        //     => this.innerReader.ReadAsDouble();
        // public override Task<double?> ReadAsDoubleAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsDoubleAsync(cancellationToken);
        // public override int? ReadAsInt32()
        //     => this.innerReader.ReadAsInt32();
        // public override Task<int?> ReadAsInt32Async(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsInt32Async(cancellationToken);
        // public override string ReadAsString() 
        //     => this.innerReader.ReadAsString();
        // public override Task<string> ReadAsStringAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsStringAsync(cancellationToken);
        // public override Task<bool> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerReader.ReadAsync(cancellationToken);

        private void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}
