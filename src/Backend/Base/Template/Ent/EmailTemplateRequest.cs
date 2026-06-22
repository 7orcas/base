namespace Backend.Base.Template.Ent
{
    public class EmailTemplateRequest
    {
        public Dictionary<string, object> Data { get; set; }

        public string Template()
        {
            return @"Hello {{name}},

                    Your order {{orderId}} has been shipped.

                    Thanks,
                    {{company}}";
        }

        public string TemplateHtml()
        {
            return @"<!DOCTYPE html>
                <html>
                <head>
                    <meta charset=""utf-8"" />
                </head>
                <body style=""font-family: Arial;"">

                    <h2>Hello {{name}},</h2>

                    <p>Your order <strong>#{{orderId}}</strong> has been shipped.</p>

                    <p>
                        Track it here:
                        <a href=""{{trackingUrl}}"">Track Order</a>
                    </p>

                    <hr />

                    <p style=""color: gray;"">
                        Thanks,<br />
                        {{company}}
                    </p>

                </body>
                </html>";
        }


    }
}
