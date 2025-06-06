CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

SET timezone = 'UTC';

DO $$
BEGIN
    RAISE NOTICE 'Base de datos EnvíosExpress inicializada correctamente';
    RAISE NOTICE 'Timezone configurado: %', current_setting('timezone');
    RAISE NOTICE 'Versión PostgreSQL: %', version();
    RAISE NOTICE 'Base de datos: %', current_database();
END $$;