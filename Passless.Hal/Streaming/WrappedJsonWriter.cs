using System;
using Newtonsoft.Json;

namespace Passless.Hal.Streaming
{
    public class WrappedJsonWriter : JsonWriter
    {
        private bool isDisposed;
        protected JsonWriter innerWriter;
        public WrappedJsonWriter(JsonWriter inner)
        {
            this.innerWriter = inner
                ?? throw new ArgumentNullException(nameof(inner));

            this.AutoCompleteOnClose = inner.AutoCompleteOnClose;
            this.Culture = inner.Culture;
            this.DateFormatHandling = inner.DateFormatHandling;
            this.DateFormatString = inner.DateFormatString;
            this.DateTimeZoneHandling = inner.DateTimeZoneHandling;
            this.FloatFormatHandling = inner.FloatFormatHandling;
            this.Formatting = inner.Formatting;
            this.StringEscapeHandling = inner.StringEscapeHandling;
        }

        public override void Close()
        {
            if (!this.isDisposed)
            {
                base.Close();
                if (this.CloseOutput)
                {
                    this.innerWriter.Close();
                    this.innerWriter = null;
                }

                this.isDisposed = true;
            }
        }
        public override void Flush()
            => this.innerWriter.Flush();

        // public override Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerWriter.FlushAsync(cancellationToken);
        public override void WriteComment(string text)
            => this.innerWriter.WriteComment(text);

        // public override Task WriteCommentAsync(string text, CancellationToken cancellationToken = default(CancellationToken))
        //     => this.innerWriter.WriteCommentAsync(text, cancellationToken);
        public override void WriteEnd()
            => this.innerWriter.WriteEnd();
        public override void WriteEndArray()
            => this.innerWriter.WriteEndArray();
        public override void WriteEndConstructor()
            => this.innerWriter.WriteEndConstructor();
        public override void WriteEndObject()
            => this.innerWriter.WriteEndObject();
        public override void WriteNull()
            => this.innerWriter.WriteNull();
        public override void WritePropertyName(string name)
            => this.innerWriter.WritePropertyName(name);
        public override void WritePropertyName(string name, bool escape)
            => this.innerWriter.WritePropertyName(name, escape);
        public override void WriteRaw(string json)
            => this.innerWriter.WriteRaw(json);
        public override void WriteRawValue(string json)
            => this.innerWriter.WriteRawValue(json);
        public override void WriteStartArray()
            => this.innerWriter.WriteStartArray();
        public override void WriteStartConstructor(string name)
            => this.innerWriter.WriteStartConstructor(name);
        public override void WriteStartObject()
            => this.innerWriter.WriteStartObject();
        public override void WriteUndefined()
            => this.innerWriter.WriteUndefined();
        public override void WriteValue(bool value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(bool? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(byte value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(byte? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(byte[] value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(char value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(char? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(DateTime value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(DateTime? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(DateTimeOffset value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(DateTimeOffset? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(decimal value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(decimal? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(double value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(double? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(float value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(float? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(Guid value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(Guid? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(int value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(int? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(long value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(long? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(object value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(sbyte value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(sbyte? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(short value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(short? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(string value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(TimeSpan value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(TimeSpan? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(uint value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(uint? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(ulong value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(ulong? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(Uri value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(ushort value)
            => this.innerWriter.WriteValue(value);
        public override void WriteValue(ushort? value)
            => this.innerWriter.WriteValue(value);
        public override void WriteWhitespace(string ws)
            => this.innerWriter.WriteWhitespace(ws);
    }
}
