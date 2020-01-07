using Elements;
using Elements.Geometry;
using Elements.Geometry.Solids;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FARLimitedMassing
{
    public static class FARLimitedMassing
    {
        /// <summary>
        /// The FARLimitedMassing function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A FARLimitedMassingOutputs instance containing computed results and the model with any new elements.</returns>
        public static FARLimitedMassingOutputs Execute(Dictionary<string, Model> inputModels, FARLimitedMassingInputs input)
        {
            //Get site(s)
            inputModels.TryGetValue("Site", out Model model);
            if (model == null)
            {
                try
                {
                    model = Model.FromJson(System.IO.File.ReadAllText("TestSite.json"));
                }
                catch (Exception e)
                {
                    throw new ArgumentException("No model found: " + e.ToString());
                }
            }
            var sites = model.AllElementsOfType<Site>();
            if (sites.Count() < 1)
            {
                throw new ArgumentException("No site found in model");
            }

            //name inputs
            var farTarget = input.FAR;
            var areaRatio = input.FootprintSize;
            var floorToFloor = input.FloorToFloorHeight;

            var facadeArea = 0.0;

            var envelopes = new List<Envelope>();
            foreach (var site in sites)
            {
                var siteBoundary = site.Perimeter;
                var siteArea = siteBoundary.Area();
                var footprintArea = siteArea * areaRatio;

                var totalProjectArea = farTarget * siteArea;

                var floorCount = Math.Floor(totalProjectArea / footprintArea);

                var scaleXform = new Transform();
                scaleXform.Scale(new Vector3(areaRatio, areaRatio, areaRatio));
                siteBoundary.Transform(scaleXform);

                var totalHeight = floorCount * floorToFloor;
                facadeArea += totalHeight * siteBoundary.Length();

                var extrusion = new Extrude(siteBoundary, totalHeight, Vector3.ZAxis, 0, false);
                var geomRep = new Representation(new List<SolidOperation>() { extrusion });
                var envMatl = new Material("envelope", new Color(0, 0, 1.0f, 0.5f), 0.0f, 0.0f);

                envelopes.Add(new Envelope(siteBoundary, 0, totalHeight, Vector3.ZAxis,
                              0.0, new Transform(0.0, 0.0, 0.0), envMatl, geomRep, Guid.NewGuid(), ""));
            }

            var output = new FARLimitedMassingOutputs(facadeArea);
            output.model.AddElements(envelopes);
            output.model.AddElements(sites);

            return output;

        }
    }
}