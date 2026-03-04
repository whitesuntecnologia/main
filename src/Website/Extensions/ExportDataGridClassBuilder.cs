using CsvHelper.Configuration.Attributes;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Website.Extensions
{
    internal class ExportDataGridClassBuilder
    {
        private readonly AssemblyName asemblyName;

        public ExportDataGridClassBuilder(string ClassName)
        {
            asemblyName = new AssemblyName(ClassName);
        }

        public class ExportDataGridClassBuilderProperty
        {
            public string? Name { get; set; }
            public Type? Type { get; set; }
            public string? Header { get; set; }
        }

        public object? CreateObject(IEnumerable<ExportDataGridClassBuilderProperty> properties)
        {
            TypeBuilder dynamicClass = CreateClass();
            CreateConstructor(dynamicClass);

            foreach (var property in properties)
            {
                CreateProperty(dynamicClass, property);
            }

            var type = dynamicClass.CreateType();

            return Activator.CreateInstance(type);
        }

        private TypeBuilder CreateClass()
        {
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(asemblyName.FullName,
                                TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout,
                                null);
            return typeBuilder;
        }

        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }

        private void CreateProperty(TypeBuilder typeBuilder, ExportDataGridClassBuilderProperty property)
        {
            var propertyName = property.Name;
            var propertyType = property.Type;
            var propertyHeader = property.Header;

            if (propertyName != null && propertyType != null && propertyHeader != null)
            {
                FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

                var nameAttributeCtr = typeof(NameAttribute).GetConstructor(new Type[] { typeof(string) });
                if(nameAttributeCtr != null)
                    propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(nameAttributeCtr, new object[] { propertyHeader }));

                var descriptionAttributeCtr = typeof(DescriptionAttribute).GetConstructor(new Type[] { typeof(string) });
                if(descriptionAttributeCtr != null)
                    propertyBuilder.SetCustomAttribute(new CustomAttributeBuilder(descriptionAttributeCtr, new object[] { propertyHeader }));

                MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
                ILGenerator getIl = getPropMthdBldr.GetILGenerator();

                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, fieldBuilder);
                getIl.Emit(OpCodes.Ret);

                MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                      MethodAttributes.Public |
                      MethodAttributes.SpecialName |
                      MethodAttributes.HideBySig,
                      null, new[] { propertyType });

                ILGenerator setIl = setPropMthdBldr.GetILGenerator();
                Label modifyProperty = setIl.DefineLabel();
                Label exitSet = setIl.DefineLabel();

                setIl.MarkLabel(modifyProperty);
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Stfld, fieldBuilder);

                setIl.Emit(OpCodes.Nop);
                setIl.MarkLabel(exitSet);
                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getPropMthdBldr);
                propertyBuilder.SetSetMethod(setPropMthdBldr);
            }
        }
    }
}
