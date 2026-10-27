using DreadsMashedPatch.AssetForwarding;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Xunit;
using Activator = Mutagen.Bethesda.Skyrim.Activator;

namespace DreadsMashedPatch.Tests;

public sealed partial class AssetForwardingPolicyTests
{

    [Fact]
    public void RegionGrassContentAndAudioReferencesForwardNormally()
    {
        var original = new Region(Key, SkyrimRelease.SkyrimSE)
        {
            Grasses = new RegionGrasses { Priority = 1, Icons = Image("a.dds"), Grasses = new ExtendedList<RegionGrass>() },
            Sounds = new RegionSounds { Priority = 1, Sounds = new ExtendedList<RegionSound>() }
        };
        original.Sounds.Music.SetTo(new FormKey(Original, 0x810));
        var earlier = original.DeepCopy();
        earlier.Grasses!.Priority = 9;
        earlier.Grasses.Grasses!.Add(new RegionGrass { Unknown = 123 });
        earlier.Grasses.Icons = Image("b.dds");
        earlier.Sounds!.Priority = 9;
        earlier.Sounds.Music.SetTo(new FormKey(Original, 0x811));
        earlier.Sounds.Sounds!.Add(new RegionSound { Chance = 25 });
        var config = new PatcherConfiguration();
        var saved = Assert.IsType<Region>(Assert.Single(Run(new RegionRecordHandler(), config, original, earlier, original.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, saved.Grasses!.Priority);
        Assert.Equal(123, Assert.Single(saved.Grasses.Grasses!).Unknown);
        Assert.Equal(9, saved.Sounds!.Priority);
        Assert.Equal(earlier.Grasses!.Icons!.LargeIconFilename, saved.Grasses.Icons!.LargeIconFilename);
        Assert.Equal(earlier.Sounds!.Music.FormKey, saved.Sounds.Music.FormKey);
        Assert.Single(saved.Sounds.Sounds!);
    }

    [Fact]
    public void DestructionAndBodyPartDataRetainSeparateResourceOwners()
    {
        var original = new Activator(Key, SkyrimRelease.SkyrimSE) { Destructible = new Destructible { Data = new DestructableData { Health = 1 } } };
        original.Destructible.Stages.Add(new DestructionStage { Model = Mesh("a.nif"), Data = new DestructionStageData { HealthPercent = 1 } });
        var earlier = original.DeepCopy();
        earlier.Destructible!.Data!.Health = 9;
        earlier.Destructible.Stages[0].Data!.HealthPercent = 9;
        earlier.Destructible.Stages[0].Model = Mesh("b.nif");
        var config = new PatcherConfiguration();
        var saved = Assert.IsType<Activator>(Assert.Single(Run(new ActivatorRecordHandler(), config, original, earlier, original.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, saved.Destructible!.Data!.Health);
        Assert.Equal(9, saved.Destructible.Stages[0].Data!.HealthPercent);
        Assert.Equal(earlier.Destructible!.Stages[0].Model!.File, saved.Destructible.Stages[0].Model!.File);

        var body = new BodyPartData(Key, SkyrimRelease.SkyrimSE);
        body.Parts.Add(new BodyPart { PartNode = "Head", DamageMult = 1, LimbReplacementModel = "a.nif", LimbReplacementScale = 1 });
        var changed = body.DeepCopy();
        changed.Parts[0].DamageMult = 9;
        changed.Parts[0].LimbReplacementModel = "b.nif";
        changed.Parts[0].LimbReplacementScale = 2;
        var patched = Assert.IsType<BodyPartData>(Assert.Single(Run(new BodyPartDataRecordHandler(), config, body, changed, body.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, patched.Parts[0].DamageMult);
        Assert.Equal("b.nif", patched.Parts[0].LimbReplacementModel);
        Assert.Equal(2, patched.Parts[0].LimbReplacementScale);
    }

    [Fact]
    public void ShaderAtlasAndImagesForwardTogetherAlongsideOrdinaryEffects()
    {
        var original = new EffectShader(Key, SkyrimRelease.SkyrimSE)
        { ParticleShaderTexture = new AssetLink<SkyrimTextureAssetType>("a.dds"), TextureCountU = 1, FillAlphaFadeInTime = 1 };
        var earlier = original.DeepCopy();
        earlier.ParticleShaderTexture = new AssetLink<SkyrimTextureAssetType>("b.dds");
        earlier.TextureCountU = 4;
        earlier.ParticleAnimatedFrameCount = 4;
        earlier.FillAlphaFadeInTime = 9;
        earlier.AmbientSound.SetTo(new FormKey(Original, 0x820));
        var config = new PatcherConfiguration();
        var saved = Assert.IsType<EffectShader>(Assert.Single(Run(new EffectShaderRecordHandler(), config, original, earlier, original.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, saved.FillAlphaFadeInTime);
        Assert.Equal(earlier.AmbientSound.FormKey, saved.AmbientSound.FormKey);
        Assert.Equal(4u, saved.TextureCountU);
        Assert.Equal(4u, saved.ParticleAnimatedFrameCount);
        Assert.Equal(earlier.ParticleShaderTexture, saved.ParticleShaderTexture);
    }

    [Fact]
    public void NestedReferenceWritesFollowStructureRegardlessOfDictionaryOrder()
    {
        var handler = new RegionRecordHandler();
        var source = new Region(Key, SkyrimRelease.SkyrimSE) { Grasses = new RegionGrasses { Icons = Image("a.dds") } };
        var target = source.DeepCopy();
        handler.ApplyForwardedProperties(target, new()
        {
            ["Grasses.Icons"] = handler.PropertyHandlers["Grasses.Icons"].GetValue(source), ["Grasses"] = null
        });
        Assert.Null(target.Grasses);

        var bodyHandler = new BodyPartDataRecordHandler();
        var body = new BodyPartData(Key, SkyrimRelease.SkyrimSE);
        body.Parts.Add(new BodyPart { PartNode = "Head", LimbReplacementModel = "a.nif" });
        body.Parts.Add(new BodyPart { PartNode = "Head", LimbReplacementModel = "b.nif" });
        var bodyTarget = body.DeepCopy();
        bodyHandler.ApplyForwardedProperties(bodyTarget, new()
        { ["Parts.LimbMeshes"] = bodyHandler.PropertyHandlers["Parts.LimbMeshes"].GetValue(body), ["Parts"] = new List<IBodyPartGetter>() });
        Assert.Empty(bodyTarget.Parts);
        // Duplicate keys retain occurrence identity, including an empty first occurrence.
        body.Parts[0].LimbReplacementModel = "";
        bodyTarget = body.DeepCopy();
        bodyTarget.Parts[1].LimbReplacementModel = "old.nif";
        bodyHandler.ApplyForwardedProperties(bodyTarget, new() { ["Parts.LimbMeshes"] = bodyHandler.PropertyHandlers["Parts.LimbMeshes"].GetValue(body) });
        Assert.Equal("", bodyTarget.Parts[0].LimbReplacementModel);
        Assert.Equal("b.nif", bodyTarget.Parts[1].LimbReplacementModel);

        var destruction = new Activator(Key, SkyrimRelease.SkyrimSE) { Destructible = new Destructible() };
        destruction.Destructible.Stages.Add(new DestructionStage { Model = Mesh("a.nif") });
        var destructionHandler = new ActivatorRecordHandler();
        var destructionTarget = destruction.DeepCopy();
        destructionHandler.ApplyForwardedProperties(destructionTarget, new()
        { ["Destructible.StageModels"] = destructionHandler.PropertyHandlers["Destructible.StageModels"].GetValue(destruction), ["Destructible"] = new Destructible() });
        Assert.Empty(destructionTarget.Destructible!.Stages);
    }

    [Fact]
    public void NonReferenceFieldsAreExcludedFromResourceSwitches()
    {
        void Excludes(AbstractRecordHandler handler, params string[] fields)
        {
            var members = AssetGroupCatalog.For(handler.GetType()).SelectMany(group => group.Members).ToHashSet();
            Assert.All(fields, field => Assert.DoesNotContain(field, members));
        }
        Excludes(new CameraShotRecordHandler(), "Duration", "Flags", "Action", "MaxTime");
        Excludes(new ImpactRecordHandler(), "Duration", "Orientation", "AngleThreshold", "PlacementRadius");
        Excludes(new BookRecordHandler(), "BookText");
        Excludes(new PerkRecordHandler(), "Level", "Conditions", "Effects", "NextPerk");
        Excludes(new QuestRecordHandler(), "Stages", "Aliases", "Objectives", "EventConditions");
        Excludes(new SceneRecordHandler(), "Phases", "Actions", "Actors", "Quest", "LastActionIndex");
        Excludes(new SoundDescriptorRecordHandler(), "String", "Pitch", "Volume", "Category", "OutputModel", "AlternateSoundFor");
        Excludes(new ShaderParticleGeometryRecordHandler(), "GravityVelocity", "ParticleDensity", "Type");
        Excludes(new MaterialObjectRecordHandler(), "DNAMs", "Flags", "HasSnow", "ProjectionVector");
        Excludes(new LensFlareRecordHandler(), "ColorInfluence", "FadeDistanceRadiusScale");
        Excludes(new CellRecordHandler(), "Water");
        Excludes(new WorldspaceRecordHandler(), "Water");
    }

    [Fact]
    public void OrdinaryTuningForwardsAlongsideResourceGroups()
    {
        var config = new PatcherConfiguration();
        var camera = new CameraShot(Key, SkyrimRelease.SkyrimSE) { Model = Mesh("a.nif"), MaxTime = 1 };
        var cameraEdit = camera.DeepCopy(); cameraEdit.Model = Mesh("b.nif"); cameraEdit.MaxTime = 9;
        var savedCamera = Assert.IsType<CameraShot>(Assert.Single(Run(new CameraShotRecordHandler(), config, camera, cameraEdit, camera.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, savedCamera.MaxTime);
        Assert.Equal(cameraEdit.Model!.File, savedCamera.Model!.File);

        var impact = new Impact(Key, SkyrimRelease.SkyrimSE) { Model = Mesh("a.nif"), Duration = 1 };
        var impactEdit = impact.DeepCopy(); impactEdit.Model = Mesh("b.nif"); impactEdit.Duration = 9;
        var savedImpact = Assert.IsType<Impact>(Assert.Single(Run(new ImpactRecordHandler(), config, impact, impactEdit, impact.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, savedImpact.Duration);
        Assert.Equal(impactEdit.Model!.File, savedImpact.Model!.File);

        var sound = Sound(false);
        var soundEdit = Sound(true); soundEdit.StaticAttenuation = 9;
        var savedSound = Assert.IsType<SoundDescriptor>(Assert.Single(Run(new SoundDescriptorRecordHandler(), config, sound, soundEdit, Sound(false)).EnumerateMajorRecords()));
        Assert.Equal(9, savedSound.StaticAttenuation);
        Assert.Equal(soundEdit.SoundFiles[0], savedSound.SoundFiles[0]);

        var particles = new ShaderParticleGeometry(Key, SkyrimRelease.SkyrimSE)
        { ParticleTexture = new AssetLink<SkyrimTextureAssetType>("a.dds"), NumSubtexturesX = 1, ParticleDensity = 1 };
        var particleEdit = particles.DeepCopy(); particleEdit.ParticleTexture = new AssetLink<SkyrimTextureAssetType>("b.dds");
        particleEdit.NumSubtexturesX = 4; particleEdit.ParticleDensity = 9;
        var savedParticles = Assert.IsType<ShaderParticleGeometry>(Assert.Single(Run(new ShaderParticleGeometryRecordHandler(), config, particles, particleEdit, particles.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, savedParticles.ParticleDensity);
        Assert.Equal(4u, savedParticles.NumSubtexturesX);

        var water = new Water(Key, SkyrimRelease.SkyrimSE)
        { NoiseLayerOneTexture = new AssetLink<SkyrimTextureAssetType>("a.dds"), NoiseLayerOneUvScale = 1, NoiseLayerOneWindSpeed = 1 };
        var waterEdit = water.DeepCopy(); waterEdit.NoiseLayerOneTexture = new AssetLink<SkyrimTextureAssetType>("b.dds");
        waterEdit.NoiseLayerOneUvScale = 4; waterEdit.NoiseLayerOneWindSpeed = 9;
        var savedWater = Assert.IsType<Water>(Assert.Single(Run(new WaterRecordHandler(), config, water, waterEdit, water.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, savedWater.NoiseLayerOneWindSpeed);
        Assert.Equal(4, savedWater.NoiseLayerOneUvScale);

        var material = new MaterialObject(Key, SkyrimRelease.SkyrimSE) { Model = Mesh("a.nif"), FalloffScale = 1 };
        var materialEdit = material.DeepCopy(); materialEdit.Model = Mesh("b.nif"); materialEdit.FalloffScale = 9;
        var savedMaterial = Assert.IsType<MaterialObject>(Assert.Single(Run(new MaterialObjectRecordHandler(), config, material, materialEdit, material.DeepCopy()).EnumerateMajorRecords()));
        Assert.Equal(9, savedMaterial.FalloffScale);
        Assert.Equal(materialEdit.Model!.File, savedMaterial.Model!.File);
    }

    [Fact]
    public void SplitResourceProjectionsCopyBinaryOverlaysAndRoundTripDetachedData()
    {
        var source = new SkyrimMod(Original, SkyrimRelease.SkyrimSE);
        var activator = new Activator(new FormKey(Original, 0x901), SkyrimRelease.SkyrimSE)
        { Destructible = new Destructible { Data = new DestructableData { Health = 99 } } };
        var stageModel = Mesh("stage.nif"); stageModel.Data = new byte[] { 1, 2 };
        activator.Destructible.Stages.Add(new DestructionStage { Model = stageModel, Data = new DestructionStageData { HealthPercent = 25 } });
        source.Activators.Add(activator);
        var body = new BodyPartData(new FormKey(Original, 0x902), SkyrimRelease.SkyrimSE);
        body.Parts.Add(new BodyPart { PartNode = "Head", DamageMult = 7, LimbReplacementModel = "head.nif", LimbReplacementScale = 2, TextureFilesHashes = new byte[] { 3, 4 } });
        source.BodyParts.Add(body);
        var region = new Region(new FormKey(Original, 0x903), SkyrimRelease.SkyrimSE)
        { Land = new RegionLand { Priority = 4, Icons = Image("land.dds") }, Sounds = new RegionSounds { Priority = 7, Sounds = new ExtendedList<RegionSound>() } };
        region.Sounds.Sounds.Add(new RegionSound { Chance = 40 });
        source.Regions.Add(region);
        var shader = new EffectShader(new FormKey(Original, 0x904), SkyrimRelease.SkyrimSE)
        { ParticleShaderTexture = new AssetLink<SkyrimTextureAssetType>("atlas.dds"), TextureCountU = 4, FillAlphaFadeInTime = 7 };
        source.EffectShaders.Add(shader);
        using var input = new MemoryStream(); source.WriteToBinary(input); input.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(input, SkyrimRelease.SkyrimSE, Original);
        var output = new SkyrimMod(Patch, SkyrimRelease.SkyrimSE);
        var cases = new (AbstractRecordHandler Handler, IMajorRecordGetter Source, IMajorRecord Target, string[] Keys)[]
        {
            (new ActivatorRecordHandler(), Assert.Single(overlay.Activators), new Activator(activator.FormKey, SkyrimRelease.SkyrimSE), ["Destructible", "Destructible.StageModels"]),
            (new BodyPartDataRecordHandler(), Assert.Single(overlay.BodyParts), new BodyPartData(body.FormKey, SkyrimRelease.SkyrimSE), ["Parts", "Parts.LimbMeshes"]),
            (new RegionRecordHandler(), Assert.Single(overlay.Regions), new Region(region.FormKey, SkyrimRelease.SkyrimSE), ["Land", "Land.Icons", "Sounds", "Sounds.Sounds"]),
            (new EffectShaderRecordHandler(), Assert.Single(overlay.EffectShaders), new EffectShader(shader.FormKey, SkyrimRelease.SkyrimSE), ["EffectShaderData", "ParticleAtlas", "ParticleShaderTexture"])
        };
        foreach (var item in cases)
        {
            item.Handler.ApplyForwardedProperties(item.Target, item.Keys.Reverse().ToDictionary(key => key, key => item.Handler.PropertyHandlers[key].GetValue(item.Source)));
            foreach (var key in item.Keys)
            {
                var handler = item.Handler.PropertyHandlers[key];
                Assert.True(handler.AreValuesEqual(handler.GetValue(item.Source), handler.GetValue(item.Target)), key);
            }
        }
        output.Activators.Add((Activator)cases[0].Target); output.BodyParts.Add((BodyPartData)cases[1].Target);
        output.Regions.Add((Region)cases[2].Target); output.EffectShaders.Add((EffectShader)cases[3].Target);
        using var written = new MemoryStream(); output.WriteToBinary(written); written.Position = 0;
        using var reload = SkyrimMod.CreateFromBinaryOverlay(written, SkyrimRelease.SkyrimSE, Patch);
        foreach (var item in cases)
        foreach (var key in item.Keys)
        {
            var handler = item.Handler.PropertyHandlers[key];
            var saved = reload.EnumerateMajorRecords().Single(record => record.FormKey == item.Source.FormKey);
            Assert.True(handler.AreValuesEqual(handler.GetValue(item.Source), handler.GetValue(saved)), key);
        }
        var modelData = ((Activator)cases[0].Target).Destructible!.Stages[0].Model!.Data!.Value; modelData[0] = 9;
        var hashes = ((BodyPartData)cases[1].Target).Parts[0].TextureFilesHashes!.Value; hashes[0] = 9;
        Assert.Equal(1, Assert.Single(overlay.Activators).Destructible!.Stages[0].Model!.Data!.Value[0]);
        Assert.Equal(3, Assert.Single(overlay.BodyParts).Parts[0].TextureFilesHashes!.Value[0]);
    }

    private static Icons Image(string path) => new() { LargeIconFilename = new AssetLink<SkyrimTextureAssetType>(path) };
}
