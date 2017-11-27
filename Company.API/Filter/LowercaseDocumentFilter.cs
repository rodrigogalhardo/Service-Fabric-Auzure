using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.API.Filter
{
    /// <summary>
    /// Torna a primeira letra dos serviços em minúscula.
    /// </summary>
    public class LowercaseDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// Aplica o filtro para tornar a primeira letra do serviço em minúscula.
        /// </summary>
        /// <param name="swaggerDoc">Documento.</param>
        /// <param name="context">Context.</param>
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths;

            var newPaths = new Dictionary<string, PathItem>();
            var removeKeys = new List<string>();

            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path.Key) ||
                    path.Key.Length < 3)
                {
                    continue;
                }

                var newKey = path.Key.Substring(0, 2);

                newKey = newKey.ToLower() + path.Key.Substring(2);

                if (newKey != path.Key)
                {
                    removeKeys.Add(path.Key);
                    newPaths.Add(newKey, path.Value);
                }
            }

            foreach (var path in newPaths)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }

            foreach (var key in removeKeys)
            {
                swaggerDoc.Paths.Remove(key);
            }
        }
    }
}
