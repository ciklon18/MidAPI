﻿using MisAPI.Entities;
using MisAPI.Enums;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using ConsultationModel = MisAPI.Models.Api.ConsultationModel;

namespace MisAPI.Mappers;

public static class Mapper
{
    public static DoctorModel MapEntityDoctorToDoctorDto(Doctor doctor)
    {
        return new DoctorModel
        {
            Id = doctor.Id,
            CreateTime = doctor.CreateTime.ToUniversalTime(),
            Name = doctor.Name,
            Birthday = doctor.Birthday.ToUniversalTime(),
            Gender = doctor.Gender,
            Email = doctor.Email,
            Phone = doctor.Phone ?? string.Empty
        };
    }

    public static PatientModel MapEntityPatientToPatientModel(Patient patient)
    {
        return new PatientModel
        {
            Birthday = patient.Birthday,
            CreateDate = patient.CreateTime,
            Gender = patient.Gender,
            Id = patient.Id,
            Name = patient.Name
        };
    }

    public static Inspection MapInspectionCreateModelToInspection(InspectionCreateModel inspectionCreateModel)
    {
        return new Inspection
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Date = inspectionCreateModel.Date,
            Anamnesis = inspectionCreateModel.Anamnesis,
            Complaints = inspectionCreateModel.Complaints,
            Treatment = inspectionCreateModel.Treatment,
            Conclusion = inspectionCreateModel.Conclusion,
            NextVisitDate = inspectionCreateModel.NextVisitDate,

            DeathDate = inspectionCreateModel.DeathDate,
            PreviousInspectionId = inspectionCreateModel.PreviousInspectionId
        };
    }

    public static Diagnosis MapDiagnosisCreateModelToDiagnosis(DiagnosisCreateModel diagnosisCreateModel)
    {
        return new Diagnosis
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Description = diagnosisCreateModel.Description,
            Type = diagnosisCreateModel.Type
        };
    }

    public static InspectionShortModel MapEntityInspectionToInspectionShortModel(Inspection inspection)
    {
        var diagnosis = inspection.Diagnoses?.FirstOrDefault(d => d.Type == DiagnosisType.Main);
        var diagnosisModel = diagnosis != null ? MapDiagnosisToDiagnosisModel(diagnosis) : null;
        return new InspectionShortModel
        {
            CreateTime = inspection.CreateTime,
            Date = inspection.Date,
            Id = inspection.Id,
            Diagnosis = diagnosisModel
        };
    }

    public static DiagnosisModel MapDiagnosisToDiagnosisModel(Diagnosis diagnosis)
    {
        return new DiagnosisModel
        {
            Code = diagnosis.Code,
            CreateTime = diagnosis.CreateTime,
            Description = diagnosis.Description,
            Id = diagnosis.Id,
            Name = diagnosis.Name,
            Type = diagnosis.Type
        };
    }

    public static InspectionPreviewModel MapEntityInspectionToInspectionPreviewModel(Inspection inspection,
        DiagnosisModel? diagnosisModel, bool hasChain, bool hasNested)
    {
        return new InspectionPreviewModel
        {
            CreateTime = inspection.CreateTime,
            Date = inspection.Date,
            Id = inspection.Id,
            Diagnosis = diagnosisModel,
            PatientId = inspection.PatientId,
            DoctorId = inspection.DoctorId,
            Conclusion = inspection.Conclusion,
            HasChain = hasChain,
            HasNested = hasNested,
            PreviousId = inspection.PreviousInspectionId ?? null,
            Patient = inspection.Patient.Name,
            Doctor = inspection.Doctor.Name
        };
    }

    private static DoctorModel MapDoctorToDoctorModel(Doctor doctor)
    {
        return new DoctorModel
        {
            Id = doctor.Id,
            CreateTime = doctor.CreateTime,
            Name = doctor.Name,
            Birthday = doctor.Birthday,
            Gender = doctor.Gender,
            Email = doctor.Email,
            Phone = doctor.Phone ?? ""
        };
    }

    private static PatientModel MapPatientToPatientModel(Patient patient)
    {
        return new PatientModel
        {
            Id = patient.Id,
            CreateDate = patient.CreateTime,
            Name = patient.Name,
            Birthday = patient.Birthday,
            Gender = patient.Gender
        };
    }

    

    private static CommentModel MapCommentToCommentModel(Comment consultationRootComment)
    {
        return new CommentModel
        {
            Id = consultationRootComment.Id,
            CreateTime = consultationRootComment.CreateTime,
            ParentId = consultationRootComment.ParentId,
            Content = consultationRootComment.Content,
            Author = MapDoctorToDoctorModel(consultationRootComment.Author),
            ModifyTime = consultationRootComment.ModifyTime
        };
    }

    public static InspectionModel MapEntityInspectionToInspectionModel(Inspection inspection, IEnumerable<DiagnosisModel> diagnoses,
        IEnumerable<InspectionConsultationModel> consultations)
    {
        return new InspectionModel
        {
            Id = inspection.Id,
            CreateTime = inspection.CreateTime,
            Date = inspection.Date,
            Anamnesis = inspection.Anamnesis ?? "",
            Complaints = inspection.Complaints ?? "",
            Treatment = inspection.Treatment ?? "",
            Conclusion = inspection.Conclusion,
            NextVisitDate = inspection.NextVisitDate,
            DeathDate = inspection.DeathDate,
            Doctor = MapDoctorToDoctorModel(inspection.Doctor),
            BaseInspectionId = inspection.BaseInspectionId,
            PreviousInspectionId = inspection.PreviousInspectionId,
            Patient = MapPatientToPatientModel(inspection.Patient),
            Diagnoses = diagnoses.ToList(),
            Consultations = consultations.ToList()
        };
    }

    public static InspectionConsultationModel MapConsultationToInspectionConsultationModel(Consultation consultation)
    {
        return new InspectionConsultationModel
        {
            Id = consultation.Id,
            CreateTime = consultation.CreateTime,
            Speciality = new SpecialityModel
            {
                Id = consultation.SpecialityId,
                Name = consultation.Speciality.Name
            },
            InspectionId = consultation.InspectionId,
            CommentsNumber = consultation.CommentsNumber,
            RootComment = MapCommentToInspectionCommentModel(consultation.RootComment)
        };
    }

    private static InspectionCommentModel MapCommentToInspectionCommentModel(Comment consultationRootComment)
    {
        return new InspectionCommentModel
        {
            Id = consultationRootComment.Id,
            CreateTime = consultationRootComment.CreateTime,
            ParentId = consultationRootComment.ParentId,
            Content = consultationRootComment.Content,
            Author = MapDoctorToDoctorModel(consultationRootComment.Author),
            ModifyTime = consultationRootComment.ModifyTime 
        };
        
    }

    public static Inspection GetUpdatedInspectionEntity(Inspection inspectionEntity, InspectionEditModel inspection)
    {
        inspectionEntity.Anamnesis = inspection.Anamnesis ?? inspectionEntity.Anamnesis;
        inspectionEntity.Complaints = inspection.Complaints;
        inspectionEntity.Treatment = inspection.Treatment;
        inspectionEntity.Conclusion = inspection.Conclusion;
        inspectionEntity.NextVisitDate = inspection.NextVisitDate?.ToUniversalTime() ?? null;
        inspectionEntity.DeathDate = inspection.DeathDate?.ToUniversalTime() ?? null;

        return inspectionEntity;
    }

    public static InspectionPreviewModel InspectionToInspectionPreviewModel(Inspection inspection)
    {
        return new InspectionPreviewModel
        {
            Id = inspection.Id,
            CreateTime = inspection.CreateTime,
            Date = inspection.Date,
            PatientId = inspection.PatientId,
            DoctorId = inspection.DoctorId,
            Diagnosis = inspection.Diagnoses?.FirstOrDefault(d => d.Type == DiagnosisType.Main) != null
                ? MapDiagnosisToDiagnosisModel(inspection.Diagnoses.First(d => d.Type == DiagnosisType.Main))
                : null,
            Conclusion = inspection.Conclusion,
            Doctor = inspection.Doctor.Name,
            Patient = inspection.Patient.Name,
            HasChain = inspection.PreviousInspectionId != null && inspection.PreviousInspectionId != Guid.Empty,
            HasNested = inspection.BaseInspectionId != null && inspection.BaseInspectionId != Guid.Empty,
            PreviousId = inspection.PreviousInspectionId ?? null
        };
    }

    public static ConsultationModel ConsultationToConsultationModel(Consultation consultation,
        Speciality consultationSpeciality, IEnumerable<Comment> comments)
    {
        return new ConsultationModel
        {
            Id = consultation.Id,
            CreateTime = consultation.CreateTime,
            InspectionId = consultation.InspectionId,
            Speciality = MapSpecialityToSpecialityModel(consultationSpeciality),
            Comments = comments.Select(MapCommentToCommentModel).ToList(),
        };
    }

    private static SpecialityModel MapSpecialityToSpecialityModel(Speciality consultationSpeciality)
    {
        return new SpecialityModel
        {
            Id = consultationSpeciality.Id,
            Name = consultationSpeciality.Name,
            CreateTime = consultationSpeciality.CreateTime
        };
    }

    public static Comment MapCommentCreateToComment(CommentCreateModel commentCreateModel, Guid doctorId,
        Guid consultationId)
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            AuthorId = doctorId,
            ConsultationId = consultationId,
            Content = commentCreateModel.Content,
            ParentId = commentCreateModel.ParentId
        };
    }

    public static Consultation MapConsultationCreateModelToConsultation(ConsultationCreateModel consultationCreateModel,
        Guid inspectionId, Guid authorId)
    {
        var rootComment = MapCommentCreateModelToComment(consultationCreateModel.Comment, inspectionId, authorId, null);
        return new Consultation
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            InspectionId = inspectionId,
            SpecialityId = consultationCreateModel.SpecialityId,
            CommentsNumber = 1,
            RootComment = rootComment,
            RootCommentId = rootComment.Id
        };
    }

    private static Comment MapCommentCreateModelToComment(InspectionCommentCreateModel comment, Guid consultationId,
        Guid authorId, Guid? parentId)
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            AuthorId = authorId,
            ConsultationId = consultationId,
            Content = comment.Content,
            ParentId = parentId,
            ModifyTime = null,
            Children = new List<Comment>()
        };
    }

    public static ConclusionAndDateValidationModel MapInspectionCreateModelToConclusionAndDateValidationModel(InspectionCreateModel inspectionCreateModel)
    {
        return new ConclusionAndDateValidationModel
        {
            Conclusion = inspectionCreateModel.Conclusion,
            DeathDate = inspectionCreateModel.DeathDate,
            NextVisitDate = inspectionCreateModel.NextVisitDate
        };
    }
    public static ConclusionAndDateValidationModel MapInspectionEditModelToConclusionAndDateValidationModel(InspectionEditModel inspectionEditModel)
    {
        return new ConclusionAndDateValidationModel
        {
            Conclusion = inspectionEditModel.Conclusion,
            DeathDate = inspectionEditModel.DeathDate,
            NextVisitDate = inspectionEditModel.NextVisitDate
        };
    }

}