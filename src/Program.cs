using System;
using System.Collections.Generic;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;


namespace fhir_cs_tutorial_01 // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private const string _fhirServer = "http://vonk.fire.ly";
        
        static void Main(string[] args)
        {
            
            var settings = new FhirClientSettings
            {
                PreferredFormat = ResourceFormat.Json,
                PreferredReturn = Prefer.ReturnMinimal
            };

            FhirClient fhirClient = new FhirClient(_fhirServer, settings);

            Bundle patientBundle = fhirClient.Search<Patient>(new string[] {"name=test"});

            

            int patientNumber = 0;
            List<string> patientsWithEncounters = new List<string>();

            while (patientBundle != null) {

                System.Console.WriteLine($"Total: {patientBundle.Total} Entry Count: {patientBundle.Entry.Count()}");

                // List each patient in the bundle
                foreach (Bundle.EntryComponent entry in patientBundle.Entry)
                {
                    
                    System.Console.WriteLine($"- Entry: {patientNumber,3}: {entry.FullUrl}");

                    if (entry.Resource != null)
                    {
                        Patient patient = (Patient)entry.Resource;
                        System.Console.WriteLine($" - ID: {patient.Id,20}");

                        if (patient.Name.Count > 0)
                        {
                            System.Console.WriteLine($" - Name: {patient.Name[0].ToString()}");
                        }

                        Bundle encounterBundle = fhirClient.Search<Encounter>(new string[]{$"patient=Patient/{patient.Id}"});

                        if(encounterBundle.Total == 0) {
                            continue;
                        }
                        patientsWithEncounters.Add(patient.Id);

                        System.Console.WriteLine($"Total: {encounterBundle.Total} Entry Count: {encounterBundle.Entry.Count()}");
                    }

                    patientNumber++;
                }

                // Get more results
                patientBundle = fhirClient.Continue(patientBundle);
            }

            
        }
    }
}
