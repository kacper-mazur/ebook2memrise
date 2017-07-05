using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Model
{
    public class Metadata
    {
    }

    public class DerivativeOf
    {
        public List<string> domains { get; set; }
        public string id { get; set; }
        public string language { get; set; }
        public List<string> regions { get; set; }
        public List<string> registers { get; set; }
        public string text { get; set; }
    }

    public class GrammaticalFeature
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Note
    {
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Pronunciation
    {
        public string audioFile { get; set; }
        public List<string> dialects { get; set; }
        public string phoneticNotation { get; set; }
        public string phoneticSpelling { get; set; }
        public List<string> regions { get; set; }
    }

    public class CrossReference
    {
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Note2
    {
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }

    public class GrammaticalFeature2
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Note3
    {
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Translation
    {
        public List<string> domains { get; set; }
        public List<GrammaticalFeature2> grammaticalFeatures { get; set; }
        public string language { get; set; }
        public List<Note3> notes { get; set; }
        public List<string> regions { get; set; }
        public List<string> registers { get; set; }
        public string text { get; set; }
    }

    public class Example
    {
        public List<string> definitions { get; set; }
        public List<string> domains { get; set; }
        public List<Note2> notes { get; set; }
        public List<string> regions { get; set; }
        public List<string> registers { get; set; }
        public List<string> senseIds { get; set; }
        public string text { get; set; }
        public List<Translation> translations { get; set; }
    }

    public class Note4
    {
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Pronunciation2
    {
        public string audioFile { get; set; }
        public List<string> dialects { get; set; }
        public string phoneticNotation { get; set; }
        public string phoneticSpelling { get; set; }
        public List<string> regions { get; set; }
    }

    public class Subsens
    {
    }

    public class GrammaticalFeature3
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Note5
    {
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Translation2
    {
        public List<string> domains { get; set; }
        public List<GrammaticalFeature3> grammaticalFeatures { get; set; }
        public string language { get; set; }
        public List<Note5> notes { get; set; }
        public List<string> regions { get; set; }
        public List<string> registers { get; set; }
        public string text { get; set; }
    }

    public class VariantForm
    {
        public List<string> regions { get; set; }
        public string text { get; set; }
    }

    public class Sens
    {
        public List<string> crossReferenceMarkers { get; set; }
        public List<CrossReference> crossReferences { get; set; }
        public List<string> definitions { get; set; }
        public List<string> domains { get; set; }
        public List<Example> examples { get; set; }
        public string id { get; set; }
        public List<Note4> notes { get; set; }
        public List<Pronunciation2> pronunciations { get; set; }
        public List<string> regions { get; set; }
        public List<string> registers { get; set; }
        public List<Subsens> subsenses { get; set; }
        public List<Translation2> translations { get; set; }
        public List<VariantForm> variantForms { get; set; }
    }

    public class VariantForm2
    {
        public List<string> regions { get; set; }
        public string text { get; set; }
    }

    public class Entry
    {
        public List<string> etymologies { get; set; }
        public List<GrammaticalFeature> grammaticalFeatures { get; set; }
        public string homographNumber { get; set; }
        public List<Note> notes { get; set; }
        public List<Pronunciation> pronunciations { get; set; }
        public List<Sens> senses { get; set; }
        public List<VariantForm2> variantForms { get; set; }
    }

    public class GrammaticalFeature4
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Note6
    {
        public string id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
    }

    public class Pronunciation3
    {
        public string audioFile { get; set; }
        public List<string> dialects { get; set; }
        public string phoneticNotation { get; set; }
        public string phoneticSpelling { get; set; }
        public List<string> regions { get; set; }
    }

    public class VariantForm3
    {
        public List<string> regions { get; set; }
        public string text { get; set; }
    }

    public class LexicalEntry
    {
        public List<DerivativeOf> derivativeOf { get; set; }
        public List<Entry> entries { get; set; }
        public List<GrammaticalFeature4> grammaticalFeatures { get; set; }
        public string language { get; set; }
        public string lexicalCategory { get; set; }
        public List<Note6> notes { get; set; }
        public List<Pronunciation3> pronunciations { get; set; }
        public string text { get; set; }
        public List<VariantForm3> variantForms { get; set; }
    }

    public class Pronunciation4
    {
        public string audioFile { get; set; }
        public List<string> dialects { get; set; }
        public string phoneticNotation { get; set; }
        public string phoneticSpelling { get; set; }
        public List<string> regions { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string language { get; set; }
        public List<LexicalEntry> lexicalEntries { get; set; }
        public List<Pronunciation4> pronunciations { get; set; }
        public string type { get; set; }
        public string word { get; set; }
    }

    public class DictionaryEntry
    {
        public Metadata metadata { get; set; }
        public List<Result> results { get; set; }
        public string Translation { get; set; }
    }
}
