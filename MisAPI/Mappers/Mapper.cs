using MisAPI.Entities;
using MisAPI.Enums;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Mappers;

public static class Mapper
{
    public static DoctorModel EntityDoctorToDoctorDto(Doctor doctor)
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

    public static PatientModel EntityPatientToPatientModel(Patient patient)
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

    public static Inspection InspectionCreateModelToInspection(InspectionCreateModel inspectionCreateModel)
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

    public static Diagnosis DiagnosisCreateModelToDiagnosis(DiagnosisCreateModel diagnosisCreateModel)
    {
        return new Diagnosis
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Description = diagnosisCreateModel.Description,
            Type = diagnosisCreateModel.Type,
        };
    }

    public static InspectionShortModel EntityInspectionToInspectionShortModel(Inspection inspection)
    {
        var diagnosis = inspection.Diagnoses?.FirstOrDefault(d => d.Type == DiagnosisType.Main);
        var diagnosisModel = diagnosis != null ? DiagnosisToDiagnosisModel(diagnosis) : null;
        return new InspectionShortModel
        {
            CreateTime = inspection.CreateTime,
            Date = inspection.Date,
            Id = inspection.Id,
            Diagnosis = diagnosisModel 
        };
    }

    public static DiagnosisModel DiagnosisToDiagnosisModel(Diagnosis diagnosis)
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

    public static InspectionPreviewModel EntityInspectionToInspectionPreviewModel(Inspection inspection, DiagnosisModel? diagnosisModel)
    {
        return new InspectionPreviewModel
        {
            CreateTime = inspection.CreateTime,
            Date = inspection.Date,
            Id = inspection.Id,
            Diagnosis = diagnosisModel,
            PatientId = inspection.PatientId,
            DoctorId = inspection.DoctorId
        };
    }

    public static DoctorModel DoctorToDoctorModel(Doctor doctor)
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

    public static PatientModel PatientToPatientModel(Patient patient)
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

    
    public static ConsultationModel ConsultationToConsultationModel(Consultation consultation)
    {
        return new ConsultationModel
        {
            Id = consultation.Id,
            CreateTime = consultation.CreateTime,
            InspectionId = consultation.InspectionId,
            Speciality = new SpecialityModel
            {
                Id = consultation.SpecialityId,
                Name = consultation.Speciality.Name
            },
            RootComment = CommentToCommentModel(consultation.RootComment),
            CommentsNumber = 0 // TODO: fix

        };
    }

    private static CommentModel CommentToCommentModel(Comment consultationRootComment)
    {
        return new CommentModel
        {
            Id = consultationRootComment.Id,
            CreateTime = consultationRootComment.CreateTime,
            ParentId = consultationRootComment.ParentId,
            Content = consultationRootComment.Content,
            Author = DoctorToDoctorModel(consultationRootComment.Author),
            ModifyTime = consultationRootComment.ModifyTime
        };
    }

    public static InspectionModel EntityInspectionToInspectionModel(Inspection inspection)
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
            Doctor = DoctorToDoctorModel(inspection.Doctor),
            BaseInspectionId = inspection.BaseInspectionId,
            PreviousInspectionId = inspection.PreviousInspectionId,
            Patient = PatientToPatientModel(inspection.Patient),
            Diagnoses = inspection.Diagnoses != null
                ? inspection.Diagnoses.Select(DiagnosisToDiagnosisModel)
                : new List<DiagnosisModel>(),
            Consultations = inspection.Consultations != null
                ? inspection.Consultations.Select(ConsultationToConsultationModel)
                : new List<ConsultationModel>()
        };
    }

    public static Inspection GetUpdatedInspectionEntity(Inspection inspectionEntity, InspectionEditModel inspection)
    {
        inspectionEntity.Anamnesis = inspection.Anamnesis;
        inspectionEntity.Complaints = inspection.Complaints;
        inspectionEntity.Treatment = inspection.Treatment;
        inspectionEntity.Conclusion = inspection.Conclusion;
        inspectionEntity.NextVisitDate = inspection.NextVisitDate;
        inspectionEntity.DeathDate = inspection.DeathDate;
        inspectionEntity.Diagnoses = inspection.Diagnoses
            .Select(DiagnosisCreateModelToDiagnosis)
            .ToList();
        
        return inspectionEntity;
    }
}